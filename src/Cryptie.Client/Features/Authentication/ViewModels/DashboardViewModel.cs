using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Messages.Services;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public class DashboardViewModel : RoutableViewModelBase
{
    private readonly MessagesService _messagesService;

    private string _newMessage = string.Empty;

    private Guid? _selectedGroup;

    public DashboardViewModel(
        IScreen hostScreen,
        MessagesService messagesService,
        ILoginState loginState // trzyma tylko TotpToken!
    ) : base(hostScreen)
    {
        // _messagesService = messagesService;
        //
        // // --- HARD‐CODED: jedyna grupa, do której wszyscy w testach dołączą ---
        // var testGroupId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        //
        // // Tworzymy lokalnie obiekt User tylko po to, by 
        // // MessagesService.ConnectToHub mógł doczytać user.Id i user.Groups.
        // var tempUser = new User
        // {
        //     Id = Guid.NewGuid(),
        //     Groups = new[] { new Group { Id = testGroupId } }
        // };
        //
        // // 1) Łączymy do hub'a
        // _messagesService.ConnectToHub(tempUser);
        //
        // // 2) Wypełniamy listę grup
        // JoinedGroups.Add(testGroupId);
        // SelectedGroup = testGroupId;
        //
        // // 3) Czyszczenie przy zmianie grupy
        // this.WhenAnyValue(x => x.SelectedGroup)
        //     .Where(g => g != null)
        //     .Subscribe(_ => ReceivedMessages.Clear());
        //
        // // 4) Polling przychodzących
        // Observable
        //     .Interval(TimeSpan.FromMilliseconds(500))
        //     .ObserveOn(RxApp.MainThreadScheduler)
        //     .Subscribe(_ => ProcessIncoming());
        //
        // // 5) Komenda wysyłki
        // SendMessageCommand = ReactiveCommand.Create(SendMessage);
    }

    // Kolekcje dla UI
    public ObservableCollection<string> ReceivedMessages { get; } = new();
    public ObservableCollection<string> SentMessages { get; } = new();
    public ObservableCollection<Guid> JoinedGroups { get; } = new();

    public Guid? SelectedGroup
    {
        get => _selectedGroup;
        set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
    }

    public string NewMessage
    {
        get => _newMessage;
        set => this.RaiseAndSetIfChanged(ref _newMessage, value);
    }

    public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }

    private void ProcessIncoming()
    {
        while (_messagesService.groupMessages.TryDequeue(out var msg))
        {
            if (msg.Id == SelectedGroup)
            {
                ReceivedMessages.Add(msg.Message);
            }
        }
    }

    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage) || SelectedGroup == null)
        {
            return;
        }

        _messagesService.SendMessageToGroup(NewMessage, SelectedGroup.Value);
        SentMessages.Add(NewMessage);
        NewMessage = string.Empty;
    }
}