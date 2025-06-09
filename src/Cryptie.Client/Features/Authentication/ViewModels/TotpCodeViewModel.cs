using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Core.Base;
using Cryptie.Client.Core.Mapping;
using Cryptie.Client.Core.Navigation;
using Cryptie.Client.Features.Authentication.Models;
using Cryptie.Client.Features.Authentication.Services;
using Cryptie.Client.Features.Authentication.State;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using MapsterMapper;
using ReactiveUI;

namespace Cryptie.Client.Features.Authentication.ViewModels;

public class TotpCodeViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly IKeychainManagerService _keychain;
    private readonly ILoginState _loginState;
    private readonly IMapper _mapper;
    private readonly IValidator<TotpRequestDto> _validator;

    public TotpCodeViewModel(
        IAuthenticationService authentication,
        IScreen hostScreen,
        IShellCoordinator coordinator,
        IValidator<TotpRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        ILoginState loginState,
        IMapper mapper,
        IKeychainManagerService keychain)
        : base(hostScreen)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;
        _keychain = keychain;
        _loginState = loginState;

        var loginResponse = _loginState.LastResponse!;
        Model.TotpToken = loginResponse.TotpToken;

        VerifyCommand = ReactiveCommand.CreateFromTask(TotpAuthAsync);

        VerifyCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);
    }

    internal TotpQrSetupModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> VerifyCommand { get; }

    private async Task TotpAuthAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<TotpRequestDto>(Model);

        await _validator.ValidateAsync(dto, cancellationToken);

        var result = await _authentication.TotpAsync(dto, cancellationToken);
        if (result == null)
        {
            ErrorMessage = "An error occurred. Please try again.";
            return;
        }

        _loginState.tokenTesting = result.Token.ToString(); // TODO: Remove it, ONLY FOR TESTING PURPOSES.
        if (!_keychain.TrySaveSessionToken(result.Token.ToString(), out var err))
        {
            ErrorMessage = err;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            _coordinator.ShowLogin();
        }

        _coordinator.ShowDashboard();
    }
}