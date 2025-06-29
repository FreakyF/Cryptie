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
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
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
        private readonly IUserState _userState;
        private CancellationTokenSource? _addFriendCts;
        private bool _disposed;
        private List<Guid> _groupIds = [];
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
            _groupService = groupService;
            _userState = deps.UserState;
            IconUri = options.Value.FontUri;

            AddFriendCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                _addFriendCts = new CancellationTokenSource();
                try
                {
                    var vm = new AddFriendViewModel(
                        hostScreen,
                        deps.FriendsService,
                        deps.Validator,
                        deps.UserState);

                    await ShowAddFriend.Handle((vm, _addFriendCts.Token));

                    await LoadGroupsAsync(_addFriendCts.Token);
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
                .Subscribe(async void (name) =>
                {
                    try
                    {
                        groupState.SelectedGroupName = name!;
                        var idx = Groups.IndexOf(name!);
                        if (idx < 0 || idx >= _groupIds.Count)
                        {
                            return;
                        }

                        var id = _groupIds[idx];
                        groupState.SelectedGroupId = id;

                        try
                        {
                            var isPrivate = await _groupService.IsGroupPrivateAsync(
                                new IsGroupPrivateRequestDto { GroupId = id });
                            groupState.IsGroupPrivate = isPrivate;
                        }
                        catch
                        {
                            groupState.IsGroupPrivate = true;
                        }
                    }
                    catch (Exception)
                    {
                        // Swallow exception: do nothing
                    }
                })
                .DisposeWith(_disposables);

            _ = LoadGroupsAsync(CancellationToken.None);
        }

        public ObservableCollection<string> Groups { get; } = new();

        public IReadOnlyList<Guid> GroupIds => _groupIds;

        public string? SelectedGroup
        {
            get => _selectedGroup;
            set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
        }

        public string IconUri { get; }

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
            var tokenString = _userState.SessionToken;
            if (string.IsNullOrWhiteSpace(tokenString)
                || !Guid.TryParse(tokenString, out var token))
                return;


            var ids = await _groupService.GetUserGroupsAsync(
                new UserGroupsRequestDto { SessionToken = token },
                cancellationToken);
            _groupIds = ids.ToList();

            var nameTasks = _groupIds
                .Select(id => _groupService
                    .GetGroupNameAsync(
                        new GetGroupNameRequestDto { GroupId = id },
                        cancellationToken)
                    .ContinueWith(t => t.Result ?? $"[{id}]",
                        TaskContinuationOptions.ExecuteSynchronously)
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