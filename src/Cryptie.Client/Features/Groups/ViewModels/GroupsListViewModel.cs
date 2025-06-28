using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.UserManagement.DTOs;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels
{
    public sealed class GroupsListViewModel : RoutableViewModelBase, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IGroupService _groupService;
        private readonly IKeychainManagerService _keychain;
        private CancellationTokenSource? _addFriendCts;
        private bool _disposed;
        private List<Guid> _groupIds = new();
        private string? _selectedGroup;

        public GroupsListViewModel(
            IScreen hostScreen,
            IConnectionMonitor connectionMonitor,
            IOptions<ClientOptions> options,
            AddFriendDependencies deps,
            IGroupService groupService,
            IGroupSelectionState groupState)
            : base(hostScreen)
        {
            _keychain = deps.KeychainManager;
            _groupService = groupService;
            IconUri = options.Value.FontUri;

            AddFriendCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _addFriendCts = new CancellationTokenSource();
                try
                {
                    var vm = new AddFriendViewModel(
                        hostScreen,
                        deps.FriendsService,
                        deps.KeychainManager,
                        deps.Validator,
                        deps.UserState);

                    await LoadGroupsAsync(_addFriendCts.Token);
                    await ShowAddFriend.Handle((vm, _addFriendCts.Token));
                }
                finally
                {
                    _addFriendCts?.Dispose();
                    _addFriendCts = null;
                }
            });

            connectionMonitor.ConnectionStatusChanged
                .Where(isConnected => !isConnected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => _addFriendCts?.Cancel())
                .DisposeWith(_disposables);

            connectionMonitor.Start();

            this.WhenAnyValue(vm => vm.SelectedGroup)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Subscribe(name =>
                {
                    groupState.SelectedGroupName = name!;
                    var idx = Groups.IndexOf(name!);
                    if (idx >= 0 && idx < _groupIds.Count)
                        groupState.SelectedGroupId = _groupIds[idx];
                })
                .DisposeWith(_disposables);

            _ = LoadGroupsAsync(CancellationToken.None);
        }

        public string? SelectedGroup
        {
            get => _selectedGroup;
            set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
        }

        public string IconUri { get; }

        /// <summary>
        /// Nazwy grup do wyświetlenia.
        /// </summary>
        public ObservableCollection<string> Groups { get; } = new();

        /// <summary>
        /// Id grup w tej samej kolejności co <see cref="Groups"/>.
        /// </summary>
        public IReadOnlyList<Guid> GroupIds => _groupIds;

        public ReactiveCommand<Unit, Unit> AddFriendCommand { get; }

        public Interaction<(AddFriendViewModel, CancellationToken), Unit> ShowAddFriend { get; }
            = new();

        public void Dispose()
        {
            if (_disposed) return;
            _disposables.Dispose();
            _addFriendCts?.Dispose();
            _addFriendCts = null;
            _disposed = true;
        }

        private async Task LoadGroupsAsync(CancellationToken cancellationToken)
        {
            if (!_keychain.TryGetSessionToken(out var tokenString, out _))
                return;
            if (!Guid.TryParse(tokenString, out var token))
                return;

            // Pobranie listy Guid-ów
            var ids = await _groupService.GetUserGroupsAsync(
                new UserGroupsRequestDto { SessionToken = token },
                cancellationToken);

            _groupIds = ids.ToList();

            // Pobranie nazw (lub fallback)
            var nameTasks = _groupIds
                .Select(id => _groupService
                    .GetGroupNameAsync(
                        new GetGroupNameRequestDto { GroupId = id },
                        cancellationToken)
                    .ContinueWith(t => t.Result ?? $"[{id}]", TaskContinuationOptions.ExecuteSynchronously)
                );

            var names = await Task.WhenAll(nameTasks);

            RxApp.MainThreadScheduler.Schedule(names, (_, list) =>
            {
                Groups.Clear();
                foreach (var name in list)
                    Groups.Add(name);
                SelectedGroup = Groups.FirstOrDefault();
                return Disposable.Empty;
            });
        }
    }
}