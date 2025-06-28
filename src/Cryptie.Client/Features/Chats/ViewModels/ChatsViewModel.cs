using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Chats.Dependencies;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Features.Chats.ViewModels;

public sealed class ChatsViewModel : RoutableViewModelBase, IDisposable
{
    private readonly ObservableAsPropertyHelper<string?> _currentGroupName;
    private readonly CompositeDisposable _disposables = new();
    private bool _isChatSettingsOpen;
    private string? _messageText;

    public ChatsViewModel(IScreen hostScreen, IConnectionMonitor connectionMonitor,
        ChatsViewModelDependencies deps)
        : base(hostScreen)
    {
        SettingsPanel = deps.SettingsPanel;

        GroupsPanel = new GroupsListViewModel(
            hostScreen,
            connectionMonitor,
            deps.Options,
            deps.AddFriendDependencies,
            deps.GroupService,
            deps.GroupState);

        Messages = [];

        _currentGroupName = deps.GroupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName);

        var canSend = this
            .WhenAnyValue(vm => vm.MessageText, txt => !string.IsNullOrWhiteSpace(txt));

        SendMessageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var gid = deps.GroupState.SelectedGroupId;
            if (gid == Guid.Empty || string.IsNullOrWhiteSpace(MessageText))
                return;

            var trimmedMessage = MessageText!.Trim();

            await deps.MessagesService.SendMessageToGroupAsync(gid, trimmedMessage);

            Messages.Add(new ChatMessageViewModel(
                message: trimmedMessage,
                isOwn: true,
                groupName: deps.GroupState.SelectedGroupName ?? string.Empty));

            MessageText = string.Empty;
        }, canSend);

        SendMessageCommand
            .ThrownExceptions
            .Subscribe()
            .DisposeWith(_disposables);

        deps.MessagesService.MessageReceived
            .Where(m => m.GroupId == deps.GroupState.SelectedGroupId)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(m =>
                Messages.Add(new ChatMessageViewModel(
                    message: m.Message.Trim(),
                    isOwn: false,
                    groupName: deps.GroupState.SelectedGroupName ?? string.Empty)))
            .DisposeWith(_disposables);

        ToggleChatSettingsCommand = ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });

        if (deps.KeychainManagerService.TryGetSessionToken(out var tok, out _)
            && Guid.TryParse(tok, out var userId))
        {
            deps.GroupState
                .WhenAnyValue(gs => gs.SelectedGroupId)
                .Where(id => id != Guid.Empty)
                .Take(1)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(async void (_) =>
                {
                    try
                    {
                        await deps.MessagesService.ConnectAsync(userId, GroupsPanel.GroupIds);
                    }
                    catch (Exception)
                    {
                        // Swallow exception: do nothing
                    }
                })
                .DisposeWith(_disposables);
        }
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

    public void Dispose() => _disposables.Dispose();
}