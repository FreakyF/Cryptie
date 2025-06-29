using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Chats.Dependencies;
using Cryptie.Client.Features.Chats.Events;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Groups.ViewModels;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.Extensions.Options;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public sealed class ChatsViewModel : RoutableViewModelBase, IDisposable
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly CompositeDisposable _disposables = new();
    private readonly Guid _sessionToken;
    private readonly Guid _userId;
    private bool _isChatSettingsOpen;
    private DateTime _lastMessageTimestamp = DateTime.MinValue;
    private string? _messageText;

    public ChatsViewModel(
        IScreen hostScreen,
        IConnectionMonitor connectionMonitor,
        ChatsViewModelDependencies deps,
        IUserState userState)
        : base(hostScreen)
    {
        _sessionToken = TryParseGuid(userState.SessionToken);
        _userId = userState.UserId ?? Guid.Empty;

        Messages = [];
        SettingsPanel = deps.SettingsPanel;

        GroupsPanel = CreateGroupsPanel(
            hostScreen,
            connectionMonitor,
            deps.Options,
            deps.AddFriendDependencies,
            deps.GroupService,
            deps.MessagesService,
            deps.GroupState);

        _currentGroupName = CreateCurrentGroupNameProperty(deps)
            .DisposeWith(_disposables);

        ToggleChatSettingsCommand = CreateToggleSettingsCommand()
            .DisposeWith(_disposables);

        SendMessageCommand = CreateSendMessageCommand(deps)
            .DisposeWith(_disposables);

        SendMessageCommand.ThrownExceptions
            .Subscribe(_ => { })
            .DisposeWith(_disposables);

        WatchGroupSelection(deps);
        WatchIncomingMessages(deps);
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

    private static Guid TryParseGuid(string? str) =>
        Guid.TryParse(str, out var g) ? g : Guid.Empty;

    private GroupsListViewModel CreateGroupsPanel(
        IScreen hostScreen,
        IConnectionMonitor monitor,
        IOptions<ClientOptions> options,
        AddFriendDependencies addFriendDeps,
        IGroupService groupService,
        IMessagesService messagesService,
        IGroupSelectionState groupState)
    {
        var panel = new GroupsListViewModel(
            hostScreen,
            monitor,
            options,
            addFriendDeps,
            groupService,
            messagesService,
            groupState);

        panel.Groups.CollectionChanged += (_, _) =>
        {
            this.RaisePropertyChanged(nameof(HasGroups));
            this.RaisePropertyChanged(nameof(HasNoGroups));
        };

        return panel;
    }

    private ObservableAsPropertyHelper<string?> CreateCurrentGroupNameProperty(
        ChatsViewModelDependencies deps) =>
        deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName);

    private ReactiveCommand<Unit, Unit> CreateToggleSettingsCommand() =>
        ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });

    private ReactiveCommand<Unit, Unit> CreateSendMessageCommand(
        ChatsViewModelDependencies deps)
    {
        var canSend = this
            .WhenAnyValue(vm => vm.MessageText,
                txt => !string.IsNullOrWhiteSpace(txt) && txt.Length <= 2000);

        return ReactiveCommand.CreateFromTask(async () =>
        {
            var gid = deps.GroupState.SelectedGroupId;
            var msg = MessageText?.Trim();
            if (gid == Guid.Empty || string.IsNullOrEmpty(msg))
                return;

            try
            {
                await deps.MessagesService
                    .SendMessageToGroupViaHttpAsync(_sessionToken, gid, msg);

                await deps.MessagesService
                    .ConnectAsync(_sessionToken, [gid]);
                await deps.MessagesService
                    .SendMessageToGroupAsync(_sessionToken, gid, msg);

                MessageText = string.Empty;

                var now = DateTime.UtcNow;
                if (now > _lastMessageTimestamp)
                {
                    _lastMessageTimestamp = now;
                    MessageBus.Current.SendMessage(
                        new ConversationBumped(gid, now));
                }
            }
            catch (Exception)
            {
                // Swallow exception: do nothing
            }
        }, canSend);
    }

    private void WatchGroupSelection(ChatsViewModelDependencies deps)
    {
        deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupId)
            .Where(gid => gid != Guid.Empty)
            .DistinctUntilChanged()
            .ObserveOn(RxApp.TaskpoolScheduler)
            .SelectMany(gid =>
                Observable.FromAsync(async () =>
                {
                    try
                    {
                        await deps.MessagesService
                            .ConnectAsync(_sessionToken, [gid]);

                        _lastMessageTimestamp = DateTime.MinValue;
                        var all = await deps.MessagesService
                            .GetGroupMessagesAsync(_sessionToken, gid);
                        if (all.Any())
                            _lastMessageTimestamp = all.Max(m => m.DateTime);

                        return all;
                    }
                    catch
                    {
                        return new List<GetGroupMessagesResponseDto.MessageDto>();
                    }
                }))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(history =>
            {
                Messages.Clear();
                var groupName = deps.GroupState.SelectedGroupName ?? "";
                foreach (var m in history)
                    Messages.Add(new ChatMessageViewModel(
                        m.Message,
                        m.SenderId == _userId,
                        groupName));
            })
            .DisposeWith(_disposables);
    }

    private void WatchIncomingMessages(ChatsViewModelDependencies deps)
    {
        deps.MessagesService.MessageReceived
            .Where(signal =>
                signal.GroupId == deps.GroupState.SelectedGroupId
                && signal.SenderId != _userId)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(signal =>
            {
                var groupName = deps.GroupState.SelectedGroupName ?? "";

                Messages.Add(new ChatMessageViewModel(
                    signal.Message,
                    isOwn: signal.SenderId != _userId,
                    groupName));

                var now = DateTime.UtcNow;
                _lastMessageTimestamp = now;
                MessageBus.Current.SendMessage(
                    new ConversationBumped(signal.GroupId, now));
            })
            .DisposeWith(_disposables);
    }
}