using System.Reactive;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Services;
using ReactiveUI;

namespace Cryptie.Client.Features.Account.ViewModels;

public class AccountViewModel : RoutableViewModelBase
{
    private readonly IKeychainManagerService _keychain;
    private readonly IShellCoordinator _shell;

    public AccountViewModel(IScreen hostScreen,
        IKeychainManagerService keychain,
        IShellCoordinator shell)
        : base(hostScreen)
    {
        _keychain = keychain;
        _shell = shell;

        SignOutCommand = ReactiveCommand.Create(ExecuteLogout);
    }

    public ReactiveCommand<Unit, Unit> SignOutCommand { get; }

    private void ExecuteLogout()
    {
        _keychain.TryClearSessionToken(out _);
        _shell.ResetAndShowLogin();
    }
}