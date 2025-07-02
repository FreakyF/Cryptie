using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Dependencies;
using Cryptie.Client.Core.Factories;
using Cryptie.Client.Core.Services;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.ViewModels;
using Cryptie.Client.Features.Dashboard.ViewModels;
using Cryptie.Client.Features.PinCode.ViewModels;
using Cryptie.Common.Features.UserManagement.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Core.Navigation;

public class ShellCoordinator(
    IViewModelFactory factory,
    IKeychainManagerService keychain,
    IUserDetailsService userDetailsService,
    IConnectionMonitor connectionMonitor,
    ShellStateDependencies stateDeps
) : IShellCoordinator
{
    public RoutingState Router { get; } = new();

    public async Task StartAsync()
    {
        if (!TryInitializeSession(out var sessionToken))
        {
            ClearUserState();
            ResetAndShowLogin();
            return;
        }

        if (!await connectionMonitor.IsBackendAliveAsync())
        {
            ResetAndShowLogin();
            return;
        }

        try
        {
            var userGuid = await GetUserGuidAsync(sessionToken);
            if (userGuid == Guid.Empty || !TryInitializeUser(userGuid))
            {
                ClearUserState();
                ResetAndShowLogin();
                return;
            }

            ShowDashboard();
        }
        catch (HttpRequestException ex) when (IsAuthError(ex))
        {
            ClearUserState();
            ResetAndShowLogin();
        }
        catch
        {
            // Swallow exception: do nothing
        }
    }

    public void ShowLogin()
    {
        NavigateTo<LoginViewModel>();
    }

    public void ResetAndShowLogin()
    {
        var vm = factory.Create<LoginViewModel>(this);

        Router
            .NavigateAndReset
            .Execute(vm)
            .Subscribe();
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

    public void ShowPinSetup()
    {
        NavigateTo<PinCodeViewModel>();
    }

    private bool TryInitializeSession(out Guid sessionToken)
    {
        sessionToken = Guid.Empty;

        if (!keychain.TryGetSessionToken(out var token, out _)
            || !Guid.TryParse(token, out sessionToken))
        {
            return false;
        }

        stateDeps.UserState.SessionToken = token;
        return true;
    }

    private async Task<Guid> GetUserGuidAsync(Guid sessionToken)
    {
        var dto = new UserGuidFromTokenRequestDto { SessionToken = sessionToken };
        var result = await userDetailsService.GetUserGuidFromTokenAsync(dto);
        return result?.Guid ?? Guid.Empty;
    }

    private bool TryInitializeUser(Guid userGuid)
    {
        stateDeps.UserState.UserId = userGuid;

        if (!keychain.TryGetPrivateKey(out var privateKey, out _))
        {
            return false;
        }

        stateDeps.UserState.PrivateKey = privateKey;
        return true;
    }

    private void ClearUserState()
    {
        keychain.TryClearSessionToken(out _);
        keychain.TryClearPrivateKey(out _);

        stateDeps.UserState.SessionToken = null;
        stateDeps.UserState.Login = null;
        stateDeps.UserState.PrivateKey = null;
        stateDeps.UserState.Username = null;
        stateDeps.UserState.UserId = null;

        stateDeps.GroupSelectionState.SelectedGroupId = Guid.Empty;
        stateDeps.GroupSelectionState.SelectedGroupName = null;
        stateDeps.GroupSelectionState.IsGroupPrivate = false;

        stateDeps.LoginState.LastResponse = null;
        stateDeps.RegistrationState.LastResponse = null;
    }

    private static bool IsAuthError(HttpRequestException ex)
    {
        return ex.StatusCode is HttpStatusCode.Unauthorized
            or HttpStatusCode.Forbidden
            or HttpStatusCode.BadRequest;
    }

    private void NavigateTo<TViewModel>() where TViewModel : RoutableViewModelBase
    {
        var vm = factory.Create<TViewModel>(this);
        Router.Navigate.Execute(vm);
    }
}