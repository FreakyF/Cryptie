using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.AddFriend.ViewModels;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels;

public sealed class GroupsListViewModel : RoutableViewModelBase, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private CancellationTokenSource? _addFriendCts;
    private bool _disposed;

    public GroupsListViewModel(IScreen hostScreen, IConnectionMonitor connectionMonitor,
        IOptions<ClientOptions> options, IFriendsService friendsService,
        IKeychainManagerService keychainManager,
        IValidator<AddFriendRequestDto> validator,
        IUserState userState) : base(hostScreen)
    {
        IconUri = options.Value.FontUri;
        AddFriendCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _addFriendCts = new CancellationTokenSource();
            try
            {
                var vm = new AddFriendViewModel(hostScreen, friendsService, keychainManager, validator, userState);
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
            .Subscribe(_ => { _addFriendCts?.Cancel(); })
            .DisposeWith(_disposables);

        connectionMonitor.Start();
    }

    public string IconUri { get; }

    public ObservableCollection<string> Friends { get; } =
    [
        "user4.dev", // idx=0  → #F44336 (red)
        "user5.dev", // idx=1  → #E91E63 (pink)
        "user6.dev", // idx=2  → #9C27B0 (purple)
        "user7.dev", // idx=3  → #673AB7 (deep purple)
        "user8.dev", // idx=4  → #3F51B5 (indigo)
        "user9.dev", // idx=5  → #2196F3 (blue)
        "user15.dev", // idx=6  → #03A9F4 (light blue)
        "user16.dev", // idx=7  → #00BCD4 (cyan)
        "user17.dev", // idx=8  → #009688 (teal)
        "user18.dev", // idx=9  → #4CAF50 (green)
        "user19.dev", // idx=10 → #090CAA (dark blue)
        "user70.dev", // idx=11 → #9CB400 (shrek lime)
        "user0.dev", // idx=12 → #09AA56 (bright green)
        "user1.dev", // idx=13 → #FFC107 (amber)
        "user2.dev", // idx=14 → #FF9800 (orange)
        "user3.dev" // idx=15 → #FF5722 (deep orange)
    ];

    public ReactiveCommand<Unit, Unit> AddFriendCommand { get; }

    public Interaction<(AddFriendViewModel, CancellationToken), Unit> ShowAddFriend { get; } = new();

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _disposables.Dispose();
            _addFriendCts?.Dispose();
            _addFriendCts = null;
        }

        _disposed = true;
    }
}