using System;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Messages.ViewModels;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public class ShellCoordinator(
    IViewModelFactory factory,
    IKeychainManagerService keychain,
    IUserDetailsService userDetailsService,
    IConnectionMonitor connectionMonitor)
    : IShellCoordinator
{
    public RoutingState Router { get; } = new();

    public async Task StartAsync()
    {
        if (keychain.TryGetSessionToken(out var token, out _) && Guid.TryParse(token, out var sessionToken))
        {
            var isConnected = await connectionMonitor.IsBackendAliveAsync();
            if (!isConnected)
            {
                return;
            }

            try
            {
                var result =
                    await userDetailsService.GetUserGuidFromTokenAsync(
                        new UserGuidFromTokenRequestDto { SessionToken = sessionToken });
                if (result is not null && result.Guid != Guid.Empty)
                {
                    ShowDashboard();
                    return;
                }
            }
            catch
            {
                // Swallowing any exceptions related to user lookup
                // to ensure the app remains stable and navigates correctly.
            }

            keychain.TryClearSessionToken(out _);
            ShowLogin();

            return;
        }

        ShowLogin();
    }

    public void ShowLogin()
    {
        NavigateTo<LoginViewModel>();
    }

    public void ShowRegister()
    {
        NavigateTo<RegisterViewModel>();
    }

    public void ShowQrSetup()
    {
        NavigateTo<TotpQrSetupViewModel>();
    }

    public void ShowTotpCode()
    {
        NavigateTo<TotpCodeViewModel>();
    }

    public void ShowDashboard()
    {
        NavigateTo<DashboardViewModel>();
    }

    private void NavigateTo<TViewModel>() where TViewModel : RoutableViewModelBase
    {
        var vm = factory.Create<TViewModel>(this);
        Router.Navigate.Execute(vm);
    }
}