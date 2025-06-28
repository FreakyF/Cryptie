using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cryptie.Client.Configuration;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Chats.Services;
using Cryptie.Client.Features.ChatSettings.ViewModels;
using Cryptie.Client.Features.Groups.Dependencies;
using Cryptie.Client.Features.Groups.Services;
using Cryptie.Client.Features.Groups.State;
using Cryptie.Client.Features.Groups.ViewModels;
using Microsoft.Extensions.Options;
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
        IOptions<ClientOptions> options,
        AddFriendDependencies deps,
        IGroupService groupService,
        IGroupSelectionState groupState,
        IKeychainManagerService keychain,
        IMessagesService messagesService,
        ChatSettingsViewModel settingsPanel)
        : base(hostScreen)
    {
        var messagesService1 = messagesService;
        var groupState1 = groupState;
        SettingsPanel = settingsPanel;

        // 1) Panel grup
        GroupsPanel = new GroupsListViewModel(
            hostScreen,
            connectionMonitor,
            options,
            deps,
            groupService,
            groupState);

        // 2) Kolekcja wiadomości
        Messages = new ObservableCollection<ChatMessageViewModel>();

        // 3) Nazwa bieżącej grupy (do nagłówka)
        _currentGroupName = groupState
            .WhenAnyValue(gs => gs.SelectedGroupName)
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, vm => vm.CurrentGroupName);

        // 4) Komenda wysyłania — tylko gdy jest jakiś tekst
        var canSend = this
            .WhenAnyValue(vm => vm.MessageText, txt => !string.IsNullOrWhiteSpace(txt));

        SendMessageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var gid = groupState1.SelectedGroupId;
            if (gid == Guid.Empty)
                return;

            // Wyślij do serwera
            await messagesService1.SendMessageToGroupAsync(gid, MessageText!);

            // Dodaj do kolekcji z przekazaniem GroupName
            Messages.Add(new ChatMessageViewModel(
                message: MessageText!,
                isOwn: true,
                groupName: groupState1.SelectedGroupName));

            MessageText = string.Empty;
        }, canSend);

        // 5) Obsługa błędów
        SendMessageCommand
            .ThrownExceptions
            .Subscribe(ex => { Console.Error.WriteLine($"Błąd wysyłania wiadomości: {ex}"); })
            .DisposeWith(_disposables);

        // 6) Odbieranie wiadomości z hubu
        messagesService1.MessageReceived
            .Where(m => m.GroupId == groupState1.SelectedGroupId)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(m =>
                Messages.Add(new ChatMessageViewModel(
                    message: m.Message,
                    isOwn: false,
                    groupName: groupState1.SelectedGroupName)))
            .DisposeWith(_disposables);

        // 7) Przełącznik panelu ustawień
        ToggleChatSettingsCommand = ReactiveCommand.Create(() => { IsChatSettingsOpen = !IsChatSettingsOpen; });

        // 8) Po wybraniu grupy: ConnectAsync
        if (keychain.TryGetSessionToken(out var tok, out _)
            && Guid.TryParse(tok, out var userId))
        {
            groupState1
                .WhenAnyValue(gs => gs.SelectedGroupId)
                .Where(id => id != Guid.Empty)
                .Take(1)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(async _ =>
                {
                    try
                    {
                        await messagesService1.ConnectAsync(userId, GroupsPanel.GroupIds);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Błąd nawiązywania połączenia: {ex}");
                    }
                })
                .DisposeWith(_disposables);
        }
    }

    /// <summary>
    /// Panel wyboru grup.
    /// </summary>
    public GroupsListViewModel GroupsPanel { get; }

    /// <summary>
    /// Kolekcja wiadomości widoczna w UI.
    /// </summary>
    public ObservableCollection<ChatMessageViewModel> Messages { get; }

    /// <summary>
    /// Tekst wpisywany w polu.
    /// </summary>
    public string? MessageText
    {
        get => _messageText;
        set => this.RaiseAndSetIfChanged(ref _messageText, value);
    }

    /// <summary>
    /// Nazwa bieżącej grupy.
    /// </summary>
    public string? CurrentGroupName => _currentGroupName.Value;

    /// <summary>
    /// Panel ustawień czatu.
    /// </summary>
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