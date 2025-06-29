using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Dependencies;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public class TotpCodeViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly TotpDependencies _deps;

    public TotpCodeViewModel(
        IAuthenticationService authentication,
        IScreen hostScreen,
        IShellCoordinator coordinator,
        TotpDependencies deps)
        : base(hostScreen)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _deps = deps;

        var loginResponse = _deps.LoginState.LastResponse!;
        Model.TotpToken = loginResponse.TotpToken;

        VerifyCommand = ReactiveCommand.CreateFromTask(TotpAuthAsync);

        VerifyCommand.ThrownExceptions
            .Select(_deps.ExceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);
    }

    internal TotpQrSetupModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }

    private async Task TotpAuthAsync(CancellationToken cancellationToken)
    {
        var dto = _deps.Mapper.Map<TotpRequestDto>(Model);

        await _deps.Validator.ValidateAsync(dto, cancellationToken);

        var result = await _authentication.TotpAsync(dto, cancellationToken);
        if (result == null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        if (!_deps.Keychain.TrySaveSessionToken(result.Token.ToString(), out var err)) ErrorMessage = err;
        _deps.UserState.SessionToken = result.Token.ToString();
        if (cancellationToken.IsCancellationRequested) _coordinator.ShowLogin();

        _coordinator.ShowDashboard();
    }
}