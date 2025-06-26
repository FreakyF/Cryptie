using System;
using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.AddFriend.ViewModels;

public class AddFriendViewModel : RoutableViewModelBase
{
    private string _friendInput = string.Empty;

    public AddFriendViewModel(IScreen hostScreen, IFriendsService friendsService,
        IKeychainManagerService keychainService)
        : base(hostScreen)
    {
        var canSend = this.WhenAnyValue(x => x.FriendInput, input => !string.IsNullOrWhiteSpace(input));

        SendFriendRequest = ReactiveCommand.CreateFromTask(async () =>
        {
            ErrorMessage = string.Empty;
            try
            {
                if (!keychainService.TryGetSessionToken(out var tokenStr, out var err) ||
                    string.IsNullOrWhiteSpace(tokenStr))
                {
                    ErrorMessage = err ?? "Session token not found!";
                    return;
                }

                if (!Guid.TryParse(tokenStr, out var sessionToken))
                {
                    ErrorMessage = "Session token is invalid!";
                    return;
                }

                var request = new AddFriendRequestDto
                {
                    SessionToken = sessionToken,
                    Friend = FriendInput
                };

                await friendsService.AddFriendAsync(request);
                FriendInput = string.Empty;
                ErrorMessage = "Friend added successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }, canSend);
    }

    public string FriendInput
    {
        get => _friendInput;
        set => this.RaiseAndSetIfChanged(ref _friendInput, value);
    }

    public ReactiveCommand<Unit, Unit> SendFriendRequest { get; }
}