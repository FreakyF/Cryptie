using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Menu.State;
using ReactiveUI;

namespace Cryptie.Client.Features.Account.ViewModels;

public class AccountViewModel : RoutableViewModelBase
{
    private readonly IKeychainManagerService _keychain;
    private readonly IShellCoordinator _shell;
    private readonly IUserState _userState;

    private string? _username;

    public AccountViewModel(
        IScreen hostScreen,
        IKeychainManagerService keychain,
        IShellCoordinator shell,
        IUserState userState)
        : base(hostScreen)
    {
        _keychain = keychain;
        _shell = shell;
        _userState = userState;

        _username = _userState.Username;

        SignOutCommand = ReactiveCommand.Create(ExecuteLogout);
    }

    public ReactiveCommand<Unit, Unit> SignOutCommand { get; }

    public string? Username
    {
        get => _username;
        set
        {
            this.RaiseAndSetIfChanged(ref _username, value);
            _userState.Username = value;
        }
    }

    private void ExecuteLogout()
    {
        _keychain.TryClearSessionToken(out _);
        _shell.ResetAndShowLogin();
    }
}