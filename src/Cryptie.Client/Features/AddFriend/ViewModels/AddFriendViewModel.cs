using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Encryption;
using Cryptie.Client.Features.AddFriend.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Common.Features.UserManagement.DTOs;
using FluentValidation;
using ReactiveUI;

namespace Cryptie.Client.Features.AddFriend.ViewModels;

public class AddFriendViewModel : RoutableViewModelBase
{
    private readonly IFriendsService _friendsService;
    private readonly IKeyService _keyService;
    private readonly IUserDetailsService _userDetailsService;
    private readonly IUserState _userState;
    private readonly IValidator<AddFriendRequestDto> _validator;
    private string _confirmationMessage = string.Empty;
    private string _encryptedAesForFriend = string.Empty;
    private string _encryptedAesForMe = string.Empty;

    private string _friendInput = string.Empty;

    public AddFriendViewModel(
        IScreen hostScreen,
        IFriendsService friendsService,
        IUserDetailsService userDetailsService,
        IKeyService keyService,
        IValidator<AddFriendRequestDto> validator,
        IUserState userState)
        : base(hostScreen)
    {
        _friendsService = friendsService;
        _keyService = keyService;
        _userDetailsService = userDetailsService;
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

    private string EncryptedAesForFriend
    {
        get => _encryptedAesForFriend;
        set => this.RaiseAndSetIfChanged(ref _encryptedAesForFriend, value);
    }

    private string EncryptedAesForMe
    {
        get => _encryptedAesForMe;
        set => this.RaiseAndSetIfChanged(ref _encryptedAesForMe, value);
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
        EncryptedAesForFriend = string.Empty;
        EncryptedAesForMe = string.Empty;

        if (!IsSessionTokenValid(_userState.SessionToken, out var sessionToken))
        {
            SetGenericError();
            return;
        }

        var currentUser = _userState.Username ?? string.Empty;
        var friendName = FriendInput.Trim();

        if (IsAddingSelf(currentUser, friendName))
        {
            ErrorMessage = "You cannot add yourself!";
            return;
        }

        var addFriendDto = new AddFriendRequestDto
        {
            SessionToken = sessionToken,
            Friend = friendName
        };

        if (!await IsValidFriendRequest(addFriendDto, ct))
        {
            ErrorMessage = "User not found!";
            return;
        }

        var friendGuid = await GetUserGuidByName(friendName, ct);
        if (friendGuid == null)
        {
            ErrorMessage = "User not found!";
            return;
        }

        var friendCert = await GetUserCertificate(friendGuid.Value, ct);
        if (friendCert == null)
        {
            SetGenericError();
            return;
        }

        var aesKey = GenerateAesKey();
        var aesKeyBase64 = Convert.ToBase64String(aesKey);

        EncryptedAesForFriend = RsaDataEncryption.Encrypt(aesKeyBase64, friendCert);

        var myGuid = await GetMyGuidFromSession(sessionToken, ct);
        if (myGuid == null)
        {
            SetGenericError();
            return;
        }

        var myCert = await GetUserCertificate(myGuid.Value, ct);
        if (myCert == null)
        {
            SetGenericError();
            return;
        }

        EncryptedAesForMe = RsaDataEncryption.Encrypt(aesKeyBase64, myCert);

        addFriendDto.EncryptionKeys = new Dictionary<Guid, string>
        {
            { friendGuid.Value, EncryptedAesForFriend },
            { myGuid.Value, EncryptedAesForMe }
        };

        if (!await TryAddFriend(addFriendDto, ct))
        {
            return;
        }

        FriendInput = string.Empty;
        ConfirmationMessage = "Friend added successfully!";
    }


    private static bool IsSessionTokenValid(string? token, out Guid sessionToken)
    {
        sessionToken = Guid.Empty;
        return !string.IsNullOrWhiteSpace(token) && Guid.TryParse(token, out sessionToken);
    }

    private static bool IsAddingSelf(string user, string friend)
    {
        return string.Equals(user, friend, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> IsValidFriendRequest(AddFriendRequestDto dto, CancellationToken ct)
    {
        return (await _validator.ValidateAsync(dto, ct)).IsValid;
    }

    private async Task<Guid?> GetUserGuidByName(string friendName, CancellationToken ct)
    {
        try
        {
            var resp = await _userDetailsService
                .GetGuidFromLoginAsync(new GuidFromLoginRequestDto { Login = friendName }, ct);
            return resp?.UserId;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<Guid?> GetMyGuidFromSession(Guid sessionToken, CancellationToken ct)
    {
        try
        {
            var resp = await _userDetailsService
                .GetUserGuidFromTokenAsync(new UserGuidFromTokenRequestDto { SessionToken = sessionToken }, ct);
            return resp?.Guid;
        }
        catch
        {
            return null;
        }
    }

    private async Task<X509Certificate2?> GetUserCertificate(Guid userGuid, CancellationToken ct)
    {
        try
        {
            var keyResp = await _keyService.GetUserKeyAsync(new GetUserKeyRequestDto { UserId = userGuid }, ct);
            if (keyResp?.PublicKey is { Length: > 0 })
            {
                return RsaDataEncryption.LoadCertificateFromBase64(keyResp.PublicKey, X509ContentType.Cert);
            }
        }
        catch
        {
            // Swallow exception: do nothing
        }

        return null;
    }

    private static byte[] GenerateAesKey()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        return aes.Key;
    }

    private async Task<bool> TryAddFriend(AddFriendRequestDto dto, CancellationToken ct)
    {
        try
        {
            await _friendsService.AddFriendAsync(dto, ct);
            return true;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            ErrorMessage = "User not found!";
        }
        catch
        {
            SetGenericError();
        }

        return false;
    }

    private void SetGenericError()
    {
        ErrorMessage = "An error occurred. Please try again.";
    }
}