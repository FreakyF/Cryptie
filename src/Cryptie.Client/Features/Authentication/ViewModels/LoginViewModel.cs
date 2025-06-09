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

public class LoginViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly ILoginState _loginState;
    private readonly IMapper _mapper;
    private readonly IValidator<LoginRequestDto> _validator;

    public LoginViewModel(
        IAuthenticationService authentication,
        IShellCoordinator coordinator,
        IValidator<LoginRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        IMapper mapper,
        ILoginState loginState)
        : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;
        _loginState = loginState;

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);

        LoginCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        GoToRegisterCommand = ReactiveCommand.Create(() => _coordinator.ShowRegister());
    }

    internal LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<LoginRequestDto>(Model);

        var validation = await _validator.ValidateAsync(dto, cancellationToken);

        if (!validation.IsValid)
        {
            ErrorMessage = "Wrong username or password.";
            return;
        }

        var result = await _authentication.LoginAsync(dto, cancellationToken);
        if (result == null)
        {
            ErrorMessage = "Wrong username or password.";
            return;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            _coordinator.ShowLogin();
        }

        _loginState.LastResponse = result;
        _coordinator.ShowTotpCode();
    }
}