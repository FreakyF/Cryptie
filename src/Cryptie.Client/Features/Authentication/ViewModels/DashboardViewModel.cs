using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.User;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public sealed class DashboardViewModel : RoutableViewModelBase, IDisposable
{
    private static readonly Guid SingleGroupId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly CancellationTokenSource _cts = new();

    private readonly ILoginState _loginState;
    private readonly MessagesService _messagesService;
    private readonly IUserManagementService _userService;

    private User? _currentUser;
    private bool _initialized;

    private string _newMessage = string.Empty;

    public DashboardViewModel(
        IScreen hostScreen,
        ILoginState loginState,
        IUserManagementService userService,
        MessagesService messagesService)
        : base(hostScreen)
    {
        _loginState = loginState;
        _userService = userService;
        _messagesService = messagesService;

        // 1) Startujemy init **od razu**
        _ = InitializeAsync();

        // 2) Komenda wysyłania jest zawsze aktywna (bez WhenActivated),
        //    ale rzuci czytelny wyjątek jeśli hub nigdy się nie połączył.
        SendMessageCommand = ReactiveCommand.Create(SendMessage);
    }

    public ObservableCollection<string> ReceivedMessages { get; } = new();
    public ObservableCollection<string> SentMessages { get; } = new();

    public string NewMessage
    {
        get => _newMessage;
        set => this.RaiseAndSetIfChanged(ref _newMessage, value);
    }

    public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private async Task InitializeAsync()
    {
        try
        {
            // --- pobranie usera
            var token = _loginState.tokenTesting
                        ?? throw new InvalidOperationException("Brak tokenu TOTP w stanie.");
            _currentUser = await _userService.GetUserAsync(token.ToString(), _cts.Token)
                           ?? throw new InvalidOperationException("Nie udało się pobrać User.");

            // --- „doklejamy” naszą testową grupę
            if (_currentUser.Groups.All(g => g.Id != SingleGroupId))
                _currentUser.Groups.Add(new Group { Id = SingleGroupId, Name = "Default" });

            // --- to jest klucz: wywołujemy ConnectToHub synchronnie/async tak, 
            // że _hubConnection powstaje zawsze
            _messagesService.ConnectToHub(_currentUser);

            // --- uruchamiamy loop odbioru
            _ = Task.Run(ReceiveLoopAsync, _cts.Token);

            _initialized = true;
        }
        catch (Exception ex)
        {
            // Jeżeli tu wpadnie — będziesz miał widoczny błąd w logach,
            // zamiast cichego "hub == null"
            Console.WriteLine($"[Dashboard Init error] {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void SendMessage()
    {
        if (!_initialized)
            throw new InvalidOperationException("Init nie powiódł się — sprawdź logi.");

        var text = NewMessage.Trim();
        if (text.Length == 0) return;

        SentMessages.Add(text);
        NewMessage = string.Empty;

        // tu już hubConnection istnieje i działa
        _messagesService.SendMessageToGroup(text, SingleGroupId);
    }

    private async Task ReceiveLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            while (_messagesService.groupMessages.TryDequeue(out var msg))
            {
                if (msg.Id == SingleGroupId)
                    await Dispatcher.UIThread.InvokeAsync(() =>
                        ReceivedMessages.Add(msg.Message));
            }

            await Task.Delay(40, _cts.Token);
        }
    }
}