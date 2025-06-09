using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Client.Features.Messages.Services;
using Cryptie.Common.Entities.Group;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public sealed class DashboardViewModel : RoutableViewModelBase, IDisposable
{
    private static readonly Guid SingleGroupId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly CancellationTokenSource _cts = new();

    private readonly ILoginState _loginState;
    private readonly MessagesService _messagesService;
    private readonly IUserManagementService _userService;

    private string _currentUserDetails = string.Empty;

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

        _ = InitializeAsync();

        SendMessageCommand = ReactiveCommand.Create(SendMessage);
    }

    public string CurrentUserDetails
    {
        get => _currentUserDetails;
        private set => this.RaiseAndSetIfChanged(ref _currentUserDetails, value);
    }

    public ObservableCollection<string> ReceivedMessages { get; } = [];
    public ObservableCollection<string> SentMessages { get; } = [];

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
        var token = _loginState.tokenTesting
                    ?? throw new InvalidOperationException("Brak tokenu TOTP w stanie.");
        var currentUser = await _userService.GetUserAsync(token, _cts.Token)
                          ?? throw new InvalidOperationException("Nie udało się pobrać User.");

        if (currentUser.Groups.All(g => g.Id != SingleGroupId))
        {
            currentUser.Groups.Add(new Group { Id = SingleGroupId, Name = "Mock Group" });
        }

        var groupNames = string.Join(
            ", ",
            currentUser.Groups
                .Select(g => g.Name)
                .DefaultIfEmpty("(None)")
        );

        var friendNames = string.Join(
            ", ",
            currentUser.Friends
                .Select(f => f.DisplayName)
                .DefaultIfEmpty("(None)")
        );

        CurrentUserDetails = new StringBuilder()
            .AppendLine("DEBUG INFO")
            .AppendLine($"Id:           {currentUser.Id}")
            .AppendLine($"Login:        {currentUser.Login}")
            .AppendLine($"DisplayName:  {currentUser.DisplayName}")
            .AppendLine($"Email:        {currentUser.Email}")
            .AppendLine($"Groups:       {groupNames}")
            .AppendLine($"Friends:      {friendNames}")
            .ToString();

        _messagesService.ConnectToHub(currentUser);
        _ = Task.Run(ReceiveLoopAsync, _cts.Token);
    }

    private void SendMessage()
    {
        var text = NewMessage.Trim();
        if (text.Length == 0) return;

        SentMessages.Add(text);
        NewMessage = string.Empty;

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