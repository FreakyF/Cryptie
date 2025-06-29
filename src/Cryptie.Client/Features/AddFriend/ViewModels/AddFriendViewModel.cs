using System;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using ReactiveUI;

namespace Cryptie.Client.Features.AddFriend.ViewModels;

public class AddFriendViewModel : RoutableViewModelBase
{
    private readonly IFriendsService _friendsService;
    private readonly IUserState _userState;
    private readonly IValidator<AddFriendRequestDto> _validator;
    private string _confirmationMessage = string.Empty;

    private string _friendInput = string.Empty;

    public AddFriendViewModel(
        IScreen hostScreen,
        IFriendsService friendsService,
        IValidator<AddFriendRequestDto> validator,
        IUserState userState)
        : base(hostScreen)
    {
        _friendsService = friendsService;
        _validator = validator;
        _userState = userState;

        var canSend = this
            .WhenAnyValue(x => x.FriendInput, input => !string.IsNullOrWhiteSpace(input));

        SendFriendRequest = ReactiveCommand.CreateFromTask(AddFriendAsync, canSend);

        this.WhenAnyValue(vm => vm.ErrorMessage)
            .Where(msg => !string.IsNullOrWhiteSpace(msg))
            .Subscribe(_ => ConfirmationMessage = string.Empty);

        this.WhenAnyValue(vm => vm.ConfirmationMessage)
            .Where(msg => !string.IsNullOrWhiteSpace(msg))
            .Subscribe(_ => ErrorMessage = string.Empty);
    }

    public string FriendInput
    {
        get => _friendInput;
        set => this.RaiseAndSetIfChanged(ref _friendInput, value);
    }

    public string ConfirmationMessage
    {
        get => _confirmationMessage;
        private set => this.RaiseAndSetIfChanged(ref _confirmationMessage, value);
    }

    public ReactiveCommand<Unit, Unit> SendFriendRequest { get; }

    private async Task AddFriendAsync(CancellationToken ct)
    {
        ErrorMessage = string.Empty;
        ConfirmationMessage = string.Empty;

        var tokenString = _userState.SessionToken;
        if (string.IsNullOrWhiteSpace(tokenString)
            || !Guid.TryParse(tokenString, out var token))
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        var currentUser = _userState.Username ?? string.Empty;
        var friendName = FriendInput.Trim();

        if (string.Equals(currentUser, friendName, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "You cannot add yourself!";
            return;
        }

        var dto = new AddFriendRequestDto
        {
            SessionToken = token,
            Friend = friendName
        };

        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
        {
            ErrorMessage = "User not found!";
            return;
        }

        try
        {
            await _friendsService.AddFriendAsync(dto, ct);

            FriendInput = string.Empty;
            ConfirmationMessage = "Friend added successfully!";
        }
        catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "User not found!";
        }
        catch
        {
            ErrorMessage = "An error occurred. Please try again.";
        }
    }
}