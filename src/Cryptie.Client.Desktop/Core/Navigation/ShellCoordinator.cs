using System;
using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Factories;
using Cryptie.Client.Desktop.Features.Authentication.Services;
using Cryptie.Client.Desktop.Features.Authentication.ViewModels;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Core.Navigation;

public class ShellCoordinator(
    IViewModelFactory factory,
    IKeychainManagerService keychain) : IShellCoordinator
{
    public RoutingState Router { get; } = new();

    public void Start()
    {
        if (keychain.TryGetSessionToken(out var token, out _) && Guid.TryParse(token, out _))
        {
            ShowDashboard();
            return;
        }

        ShowRegister();
    }

    public void ShowLogin() => NavigateTo<LoginViewModel>();
    public void ShowRegister() => NavigateTo<RegisterViewModel>();
    public void ShowQrSetup() => NavigateTo<TotpQrSetupViewModel>();
    public void ShowTotpCode() => NavigateTo<TotpCodeViewModel>();
    public void ShowDashboard() => NavigateTo<DashboardViewModel>();

    private void NavigateTo<TViewModel>() where TViewModel : RoutableViewModelBase
    {
        var vm = factory.Create<TViewModel>(this);
        Router.Navigate.Execute(vm);
    }
}