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
using Cryptie.Client.Features.Chats.Events;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.GroupManagement;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Groups.ViewModels;

public sealed class GroupsListViewModel : RoutableViewModelBase, IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IGroupService _groupService;
    private readonly IMessagesService _messagesService;
    private readonly IUserState _userState;
    private CancellationTokenSource? _addFriendCts;
    private bool _disposed;

    private List<Guid> _groupIds = [];
    private Dictionary<Guid, bool> _groupPrivacyCache = new();
    private string? _selectedGroup;

    public GroupsListViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        IOptions<ClientOptions> options,
        AddFriendDependencies deps,
        IGroupService groupService,
        IMessagesService messagesService,
        IGroupSelectionState groupState)
        : base(hostScreen)
    {
        _groupService = groupService;
        _messagesService = messagesService;
        _userState = deps.UserState;
        IconUri = options.Value.FontUri;

        MessageBus.Current
            .Listen<ConversationBumped>()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(evt =>
            {
                var idx = _groupIds.IndexOf(evt.GroupId);
                if (idx <= 0 || idx >= Groups.Count) return;

                var isCurrent = groupState.SelectedGroupId == evt.GroupId;
                var id = _groupIds[idx];
                var name = Groups[idx];

                _groupIds.RemoveAt(idx);
                Groups.RemoveAt(idx);
                _groupIds.Insert(0, id);
                Groups.Insert(0, name);

                if (isCurrent)
                    SelectedGroup = name;
            })
            .DisposeWith(_disposables);

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

        AddFriendCommand.ThrownExceptions
            .Subscribe()
            .DisposeWith(_disposables);

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
                var idx = Groups.IndexOf(name!);
                if (idx < 0 || idx >= _groupIds.Count) return;

                var id = _groupIds[idx];
                groupState.SelectedGroupName = name!;
                groupState.SelectedGroupId = id;
                groupState.IsGroupPrivate =
                    !_groupPrivacyCache.TryGetValue(id, out var isPriv) || isPriv;
            })
            .DisposeWith(_disposables);

        _ = LoadGroupsSafeAsync(CancellationToken.None);
    }

    public ObservableCollection<string> Groups { get; } = new();

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
        _disposed = true;
    }

    private async Task LoadGroupsSafeAsync(CancellationToken ct)
    {
        try
        {
            await LoadGroupsAsync(ct);
        }
        catch
        {
            // swallow
        }
    }

    private async Task LoadGroupsAsync(CancellationToken cancellationToken)
    {
        var tokenString = _userState.SessionToken;
        if (string.IsNullOrWhiteSpace(tokenString)
            || !Guid.TryParse(tokenString, out var sessionToken))
            return;

        var namesMap = await _groupService.GetGroupsNamesAsync(
            new GetGroupsNamesRequestDto { SessionToken = sessionToken },
            cancellationToken);

        _groupIds = namesMap.Keys.ToList();

        _groupPrivacyCache = await _groupService
            .GetGroupsPrivacyAsync(
                new IsGroupsPrivateRequestDto { GroupIds = _groupIds },
                cancellationToken)
            .ConfigureAwait(false);

        var lastInfos = await Task.WhenAll(_groupIds.Select(async gid =>
        {
            var history = await _messagesService
                .GetGroupMessagesAsync(sessionToken, gid)
                .ConfigureAwait(false);
            var last = history.Any()
                ? history.Max(m => m.DateTime)
                : DateTime.MinValue;
            return (GroupId: gid, Last: last);
        }));

        var previously = SelectedGroup;
        var sorted = lastInfos
            .OrderByDescending(x => x.Last)
            .Select(x => x.GroupId)
            .ToList();

        _groupIds = sorted;
        var sortedNames = sorted
            .Select(gid => namesMap.TryGetValue(gid, out var nm) ? nm : $"[{gid}]")
            .ToList();

        RxApp.MainThreadScheduler.Schedule(sortedNames, (_, list) =>
        {
            Groups.Clear();
            foreach (var nm in list)
                Groups.Add(nm);

            if (!string.IsNullOrEmpty(previously) && Groups.Contains(previously))
                SelectedGroup = previously;
            else
                SelectedGroup = Groups.FirstOrDefault();

            return Disposable.Empty;
        });
    }
}