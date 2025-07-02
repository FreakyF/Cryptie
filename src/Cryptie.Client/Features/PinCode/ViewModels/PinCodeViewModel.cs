using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Encryption;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.PinCode.ViewModels;

public class PinCodeViewModel : RoutableViewModelBase, IActivatableViewModel
{
    private readonly IKeychainManagerService _keychainManagerService;
    private readonly IShellCoordinator _shellCoordinator;
    private readonly IUserDetailsService _userDetailsService;
    private readonly IUserState _userState;

    private UserPrivateKeyResponseDto? _backendResponse;

    private string _pinCode = string.Empty;

    public PinCodeViewModel(
        IScreen hostScreen,
        IShellCoordinator shellCoordinator,
        IUserState userState,
        IKeychainManagerService keychainManagerService,
        IUserDetailsService userDetailsService
    ) : base(hostScreen)
    {
        _shellCoordinator = shellCoordinator;
        _userState = userState;
        _keychainManagerService = keychainManagerService;
        _userDetailsService = userDetailsService;

        VerifyCommand = ReactiveCommand.CreateFromTask(VerifyPinAsync);

        this.WhenActivated(disposables =>
        {
            _keychainManagerService.TryClearPrivateKey(out _);

            LoadBackendDataAsync()
                .ToObservable()
                .Subscribe()
                .DisposeWith(disposables);
        });
    }

    public string PinCode
    {
        get => _pinCode;
        set => this.RaiseAndSetIfChanged(ref _pinCode, value);
    }

    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }

    public ViewModelActivator Activator { get; } = new();

    private async Task LoadBackendDataAsync()
    {
        if (string.IsNullOrWhiteSpace(_userState.SessionToken)
            || !Guid.TryParse(_userState.SessionToken, out var sessionGuid))
        {
            return;
        }

        var requestDto = new UserPrivateKeyRequestDto
        {
            SessionToken = sessionGuid
        };

        _backendResponse = await _userDetailsService
            .GetUserPrivateKeyAsync(requestDto, CancellationToken.None)
            .ConfigureAwait(false);
    }

    private Task VerifyPinAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(PinCode) || PinCode.Length != 6)
        {
            ErrorMessage = "PIN must be exactly 6 digits.";
            return Task.CompletedTask;
        }

        if (_backendResponse is null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return Task.CompletedTask;
        }

        var aesKeyBase64 = DeriveAesKeyFromPin(PinCode);

        string? decryptedControl;
        try
        {
            decryptedControl = AesDataEncryption.Decrypt(
                _backendResponse.ControlValue,
                aesKeyBase64);
        }
        catch
        {
            decryptedControl = null;
        }

        if (decryptedControl != _userState.Login)
        {
            ErrorMessage = "PIN is incorrect. Please try again";
            return Task.CompletedTask;
        }

        string decryptedPrivateKey;
        try
        {
            decryptedPrivateKey = AesDataEncryption.Decrypt(
                _backendResponse.PrivateKey,
                aesKeyBase64);
        }
        catch
        {
            ErrorMessage = "An error occurred. Please try again.";
            return Task.CompletedTask;
        }

        if (!_keychainManagerService.TrySavePrivateKey(decryptedPrivateKey, out _))
        {
            ErrorMessage = "An error occurred. Please try again.";
            return Task.CompletedTask;
        }

        _userState.PrivateKey = decryptedPrivateKey;
        _shellCoordinator.ShowDashboard();
        return Task.CompletedTask;
    }

    private static string DeriveAesKeyFromPin(string pin)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(hash);
    }
}