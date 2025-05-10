using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Mapping;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using MapsterMapper;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IShellCoordinator _coordinator;
    private readonly IValidator<LoginRequestDto> _validator;
    private readonly IMapper _mapper;

    public LoginViewModel(
        IAuthenticationService authentication,
        IShellCoordinator coordinator,
        IValidator<LoginRequestDto> validator,
        IExceptionMessageMapper exceptionMapper,
        IMapper mapper)
        : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;
        _mapper = mapper;

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);

        LoginCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        GoToRegisterCommand = ReactiveCommand.Create(coordinator.ShowRegister);
    }

    internal LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<LoginRequestDto>(Model);

        await _validator.ValidateAsync(dto, cancellationToken);

        await _authentication.LoginAsync(dto, cancellationToken);
        
        if (!cancellationToken.IsCancellationRequested)
        {
            _coordinator.ShowLogin();
        }

        _coordinator.ShowRegister();
    }
}