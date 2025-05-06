using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Client.Desktop.Coordinators;
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
        IValidator<LoginRequestDto> validator)
        : base(coordinator)
    {
        LoginCommand = ReactiveCommand.CreateFromTask(async cancellationToken =>
        {
            var dto = new LoginRequestDto
            {
                Login = Model.Username,
                Password = Model.Password
            };

            var validation = await validator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
            {
                throw new BadCredentialsException();
            }

            await authentication.LoginAsync(dto, cancellationToken);
            coordinator.ShowRegister();
        });

        LoginCommand.ThrownExceptions
            .Select(MapExceptionToMessage)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(msg => ErrorMessage = msg);

        GoToRegisterCommand = ReactiveCommand.Create(coordinator.ShowRegister);
    }

    private static string MapExceptionToMessage(Exception exception) =>
        exception switch
        {
            BadCredentialsException => "Wrong username or password",
            HttpRequestException http when (int?)http.StatusCode == 400 => "Wrong username or password",
            OperationCanceledException => string.Empty,
            _ => exception.Message,
        };
}

public class BadCredentialsException : Exception;