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
using Cryptie.Client.Encryption;
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

public sealed class ChatsViewModel : RoutableViewModelBase, IActivatableViewModel, IDisposable
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly ChatsViewModelDependencies _deps;
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
        Activator = new ViewModelActivator();
        _deps = deps;

        _sessionToken = TryParseGuid(userState.SessionToken);
        _userId = userState.UserId ?? Guid.Empty;

        Messages = [];
        SettingsPanel = _deps.SettingsPanel;

        GroupsPanel = CreateGroupsPanel(
            hostScreen,
            connectionMonitor,
            _deps.Options,
            _deps.AddFriendDependencies,
            _deps.GroupService,
            _deps.MessagesService,
            _deps.GroupState);

        _currentGroupName = CreateCurrentGroupNameProperty()
            .DisposeWith(_disposables);

        ToggleChatSettingsCommand = CreateToggleSettingsCommand()
            .DisposeWith(_disposables);

        SendMessageCommand = CreateSendMessageCommand()
            .DisposeWith(_disposables);

        SendMessageCommand.ThrownExceptions
            .Subscribe(_ => { })
            .DisposeWith(_disposables);

        WatchGroupSelection();
        WatchIncomingMessages();
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

    public ViewModelActivator Activator { get; }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private static Guid TryParseGuid(string? str)
    {
        return Guid.TryParse(str, out var g) ? g : Guid.Empty;
    }

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
            groupState,
            addFriendDeps.KeyService);

        panel.Groups.CollectionChanged += (_, _) =>
        {
            this.RaisePropertyChanged(nameof(HasGroups));
            this.RaisePropertyChanged(nameof(HasNoGroups));
        };

        return panel;
    }

    private ObservableAsPropertyHelper<string?> CreateCurrentGroupNameProperty()
    {
        return _deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName);
    }

    private ReactiveCommand<Unit, Unit> CreateToggleSettingsCommand()
    {
        return ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });
    }

    private ReactiveCommand<Unit, Unit> CreateSendMessageCommand()
    {
        var canSend = this
            .WhenAnyValue(vm => vm.MessageText,
                txt => !string.IsNullOrWhiteSpace(txt) && txt.Length <= 2000);

        return ReactiveCommand.CreateFromTask(async () =>
        {
            var gid = _deps.GroupState.SelectedGroupId;
            var msg = MessageText?.Trim();
            if (gid == Guid.Empty || string.IsNullOrEmpty(msg))
            {
                return;
            }

            if (!GroupsPanel.GroupKeyCache.TryGetValue(gid, out var groupKey))
            {
                throw new InvalidOperationException("User's private key is missing");
            }

            var encryptedMsg = AesDataEncryption.Encrypt(msg, groupKey);

            try
            {
                await _deps.MessagesService
                    .SendMessageToGroupViaHttpAsync(_sessionToken, gid, encryptedMsg);

                await _deps.MessagesService
                    .ConnectAsync(_sessionToken, [gid]);
                await _deps.MessagesService
                    .SendMessageToGroupAsync(_sessionToken, gid, encryptedMsg);

                MessageText = string.Empty;

                var now = DateTime.UtcNow;
                if (now > _lastMessageTimestamp)
                {
                    _lastMessageTimestamp = now;
                    MessageBus.Current.SendMessage(
                        new ConversationBumped(gid, now));
                }
            }
            catch
            {
                // Swallow exception
            }
        }, canSend);
    }

    private void DecryptMessages(IEnumerable<GetGroupMessagesResponseDto.MessageDto> messages, Guid groupId)
    {
        if (!GroupsPanel.GroupKeyCache.TryGetValue(groupId, out var key))
        {
            return;
        }

        foreach (var message in messages)
        {
            message.Message = AesDataEncryption.Decrypt(message.Message, key);
        }
    }

    private void WatchGroupSelection()
    {
        _deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupId)
            .Where(gid => gid != Guid.Empty)
            .StartWith(_deps.GroupState.SelectedGroupId)
            .SelectMany(gid =>
                Observable.FromAsync(async () =>
                {
                    try
                    {
                        await GroupsPanel.KeysLoaded;

                        await _deps.MessagesService.ConnectAsync(_sessionToken, [gid]);
                        _lastMessageTimestamp = DateTime.MinValue;
                        var all = await _deps.MessagesService.GetGroupMessagesAsync(_sessionToken, gid);

                        if (all.Any())
                        {
                            _lastMessageTimestamp = all.Max(m => m.DateTime);
                        }

                        DecryptMessages(all, gid);

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
                var groupName = _deps.GroupState.SelectedGroupName ?? "";
                foreach (var m in history)
                {
                    Messages.Add(new ChatMessageViewModel(
                        m.Message,
                        m.SenderId == _userId,
                        groupName));
                }
            })
            .DisposeWith(_disposables);
    }

    private void WatchIncomingMessages()
    {
        _deps.MessagesService.MessageReceived
            .Where(signal =>
                signal.GroupId == _deps.GroupState.SelectedGroupId
                && signal.SenderId != _userId)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(signal =>
            {
                var decrypted = GroupsPanel.GroupKeyCache.TryGetValue(signal.GroupId, out var key)
                    ? AesDataEncryption.Decrypt(signal.Message, key)
                    : signal.Message;

                var groupName = _deps.GroupState.SelectedGroupName ?? "";

                Messages.Add(new ChatMessageViewModel(
                    decrypted,
                    true,
                    groupName));

                var now = DateTime.UtcNow;
                _lastMessageTimestamp = now;
                MessageBus.Current.SendMessage(
                    new ConversationBumped(signal.GroupId, now));
            })
            .DisposeWith(_disposables);
    }
}