using System;
using System.Reactive;
using System.Reactive.Linq;
using Cryptie.Client.Desktop.Core.Base;
using Cryptie.Client.Desktop.Core.Navigation;
using Cryptie.Client.Desktop.Features.Authentication.Models;
using Cryptie.Client.Desktop.Mappers;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using FluentValidation;
using ReactiveUI;

namespace Cryptie.Client.Desktop.Features.Authentication.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    public LoginViewModel(
        IAuthenticationService authentication,
        IShellCoordinator coordinator,
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

    internal LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }
}