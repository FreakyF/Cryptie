using System;
using System.Reactive;
using System.Reactive.Linq;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Client.Desktop.Coordinators;
using Cryptie.Client.Desktop.Mappers;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    internal LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    public LoginViewModel(
        IAuthenticationService authentication,
        IAppCoordinator coordinator,
        IValidator<LoginRequestDto> validator,
        IExceptionMessageMapper exceptionMapper)
        : base(coordinator)
    {
        LoginCommand = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var dto = new LoginRequestDto
            {
                Login = Model.Username,
                Password = Model.Password
            };

            await validator.ValidateAsync(dto, cancellationToken);

            await authentication.LoginAsync(dto, cancellationToken);
            coordinator.ShowRegister();
        });

        LoginCommand.ThrownExceptions
            .Select(exceptionMapper.Map)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        GoToRegisterCommand = ReactiveCommand.Create(coordinator.ShowRegister);
    }
}