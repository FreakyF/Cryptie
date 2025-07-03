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

    /// <summary>
    ///     Initializes the application routing based on the persisted session.
    /// </summary>
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

    /// <summary>
    ///     Navigates to the login view.
    /// </summary>
    public void ShowLogin()
    {
        NavigateTo<LoginViewModel>();
    }

    /// <summary>
    ///     Clears the navigation stack and shows the login screen.
    /// </summary>
    public void ResetAndShowLogin()
    {
        var vm = factory.Create<LoginViewModel>(this);

        Router
            .NavigateAndReset
            .Execute(vm)
            .Subscribe();
    }

    /// <summary>
    ///     Navigates to the registration view.
    /// </summary>
    public void ShowRegister()
    {
        NavigateTo<RegisterViewModel>();
    }

    /// <summary>
    ///     Navigates to the TOTP QR setup view.
    /// </summary>
    public void ShowQrSetup()
    {
        NavigateTo<TotpQrSetupViewModel>();
    }

    /// <summary>
    ///     Navigates to the TOTP code view.
    /// </summary>
    public void ShowTotpCode()
    {
        NavigateTo<TotpCodeViewModel>();
    }

    /// <summary>
    ///     Navigates to the dashboard view.
    /// </summary>
    public void ShowDashboard()
    {
        NavigateTo<DashboardViewModel>();
    }

    /// <summary>
    ///     Navigates to the pin code setup view.
    /// </summary>
    public void ShowPinSetup()
    {
        NavigateTo<PinCodeViewModel>();
    }

    /// <summary>
    ///     Attempts to read the persisted session token from the keychain.
    /// </summary>
    /// <param name="sessionToken">Outputs the parsed session token when successful.</param>
    /// <returns><c>true</c> if a valid token was retrieved.</returns>
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

    /// <summary>
    ///     Gets the user's GUID associated with the provided session token.
    /// </summary>
    /// <param name="sessionToken">Valid session token.</param>
    /// <returns>User GUID or <see cref="Guid.Empty"/> when not found.</returns>
    private async Task<Guid> GetUserGuidAsync(Guid sessionToken)
    {
        var dto = new UserGuidFromTokenRequestDto { SessionToken = sessionToken };
        var result = await userDetailsService.GetUserGuidFromTokenAsync(dto);
        return result?.Guid ?? Guid.Empty;
    }

    /// <summary>
    ///     Loads the current user's private key from the keychain and populates user state.
    /// </summary>
    /// <param name="userGuid">Identifier of the authenticated user.</param>
    /// <returns><c>true</c> when the key was successfully loaded.</returns>
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

    /// <summary>
    ///     Clears any cached authentication information from state and keychain.
    /// </summary>
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

    /// <summary>
    ///     Determines whether the HTTP exception represents an authentication failure.
    /// </summary>
    private static bool IsAuthError(HttpRequestException ex)
    {
        return ex.StatusCode is HttpStatusCode.Unauthorized
            or HttpStatusCode.Forbidden
            or HttpStatusCode.BadRequest;
    }

    /// <summary>
    ///     Helper method to create and navigate to a view model instance.
    /// </summary>
    private void NavigateTo<TViewModel>() where TViewModel : RoutableViewModelBase
    {
        var vm = factory.Create<TViewModel>(this);
        Router.Navigate.Execute(vm);
    }
}