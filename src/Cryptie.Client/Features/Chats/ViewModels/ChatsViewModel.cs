using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Chats.Dependencies;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.Menu.State;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public sealed class ChatsViewModel : RoutableViewModelBase, IDisposable
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly CompositeDisposable _disposables = new();
    private bool _isChatSettingsOpen;
    private string? _messageText;

    public ChatsViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        ChatsViewModelDependencies deps,
        IUserState userState)
        : base(hostScreen)
    {
        if (!Guid.TryParse(userState.SessionToken, out var sessionUserId))
            sessionUserId = Guid.Empty;

        if (!Guid.TryParse(userState.UserId?.ToString(), out var userId))
        {
            userId = Guid.Empty;
        }

        Messages = [];
        SettingsPanel = deps.SettingsPanel;
        GroupsPanel = new GroupsListViewModel(
            hostScreen,
            connectionMonitor,
            deps.Options,
            deps.AddFriendDependencies,
            deps.GroupService,
            deps.GroupState);
        GroupsPanel.Groups.CollectionChanged += OnGroupsChanged;

        _currentGroupName = deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName);

        ToggleChatSettingsCommand = ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; })
            .DisposeWith(_disposables);

        deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupId)
            .DistinctUntilChanged()
            .Where(gid => gid != Guid.Empty)
            .SelectMany(async gid => await deps.MessagesService.GetGroupMessagesAsync(
                userToken: sessionUserId,
                groupId: gid))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(history =>
            {
                Messages.Clear();
                var groupName = deps.GroupState.SelectedGroupName ?? string.Empty;
                foreach (var m in history)
                {
                    var isOwn = m.SenderId == userId;
                    Messages.Add(new ChatMessageViewModel(
                        message: m.Message,
                        isOwn: isOwn,
                        groupName: groupName));
                }
            })
            .DisposeWith(_disposables);

        var canSend = this
            .WhenAnyValue(vm => vm.MessageText,
                txt => !string.IsNullOrWhiteSpace(txt) && txt.Length <= 2000);

        SendMessageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var gid = deps.GroupState.SelectedGroupId;
                if (gid == Guid.Empty) return;

                var trimmed = MessageText!.Trim();
                if (string.IsNullOrEmpty(trimmed)) return;

                await deps.MessagesService.SendMessageToGroupViaHttpAsync(
                    senderToken: sessionUserId,
                    groupId: gid,
                    message: trimmed);

                MessageText = string.Empty;
            }, canSend)
            .DisposeWith(_disposables);

        deps.MessagesService.MessageReceived
            .Where(m => m.GroupId == deps.GroupState.SelectedGroupId)
            .SelectMany(async signal =>
            {
                if (!Guid.TryParse(signal.Message, out var msgId))
                    return null;
                return await deps.MessagesService.GetMessageFromGroupViaHttpAsync(
                    userToken: sessionUserId,
                    groupId: signal.GroupId,
                    messageId: msgId);
            })
            .Where(dto => dto != null)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(dto =>
            {
                var isOwn = dto!.SenderId == userId;
                Messages.Add(new ChatMessageViewModel(
                    message: dto.Message,
                    isOwn: isOwn,
                    groupName: deps.GroupState.SelectedGroupName ?? string.Empty));
            })
            .DisposeWith(_disposables);

        deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupId)
            .Where(id => id != Guid.Empty)
            .Take(1)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Subscribe(async void (_) =>
            {
                try
                {
                    await deps.MessagesService.ConnectAsync(
                        sessionUserId,
                        GroupsPanel.GroupIds);
                }
                catch (Exception)
                {
                    // Swallow exception: do nothing
                }
            })
            .DisposeWith(_disposables);
    }

    public GroupsListViewModel GroupsPanel { get; }
    public ObservableCollection<ChatMessageViewModel> Messages { get; }

    public string? MessageText
    {
        get => _messageText;
        set => this.RaiseAndSetIfChanged(ref _messageText, value);
    }

    public string? CurrentGroupName => _currentGroupName.Value;
    public ChatSettingsViewModel SettingsPanel { get; }
    public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleChatSettingsCommand { get; }

    public bool IsChatSettingsOpen
    {
        get => _isChatSettingsOpen;
        set => this.RaiseAndSetIfChanged(ref _isChatSettingsOpen, value);
    }

    public bool HasGroups => GroupsPanel.Groups.Count > 0;
    public bool HasNoGroups => !HasGroups;

    public void Dispose() => _disposables.Dispose();

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(HasGroups));
        this.RaisePropertyChanged(nameof(HasNoGroups));
    }
}