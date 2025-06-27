using System;
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
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels;

public sealed class GroupsListViewModel : RoutableViewModelBase, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IGroupService _groupService;
    private readonly IKeychainManagerService _keychain;
    private CancellationTokenSource? _addFriendCts;
    private bool _disposed;

    public GroupsListViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        IOptions<ClientOptions> options,
        IFriendsService friendsService,
        IKeychainManagerService keychainManager,
        IValidator<AddFriendRequestDto> validator,
        IGroupService groupService,
        IUserState userState) : base(hostScreen)
    {
        _keychain = keychainManager;
        _groupService = groupService;

        IconUri = options.Value.FontUri;
        AddFriendCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _addFriendCts = new CancellationTokenSource();
            try
            {
                var vm = new AddFriendViewModel(hostScreen, friendsService, keychainManager, validator, userState);

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

        _ = LoadGroupsAsync(CancellationToken.None);
    }

    public string IconUri { get; }

    public ObservableCollection<string> Groups { get; } = [];

    public ReactiveCommand<Unit, Unit> AddFriendCommand { get; }

    public Interaction<(AddFriendViewModel, CancellationToken), Unit> ShowAddFriend { get; } = new();

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

        var ids = await _groupService.GetUserGroupsAsync(
            new UserGroupsRequestDto { SessionToken = token },
            cancellationToken);

        var nameTasks = ids.Select(id =>
            _groupService.GetGroupNameAsync(
                    new GetGroupNameRequestDto { GroupId = id },
                    cancellationToken)
                .ContinueWith(t => t.Result ?? $"[{id}]",
                    TaskContinuationOptions.ExecuteSynchronously));

        var names = await Task.WhenAll(nameTasks);

        RxApp.MainThreadScheduler.Schedule(names, (_, list) =>
        {
            Groups.Clear();
            foreach (var name in list) Groups.Add(name);
            return Disposable.Empty;
        });
    }
}