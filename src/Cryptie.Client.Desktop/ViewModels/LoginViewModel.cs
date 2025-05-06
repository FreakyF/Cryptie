using System;
using System.Net.Http;
using System.Reactive;
using System.Security.Authentication;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using Cryptie.Client.Desktop.Coordinators;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;


namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IAppCoordinator _coordinator;
    private readonly IValidator<LoginRequestDto> _validator;

    internal LoginModel Model { get; } = new();

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    public LoginViewModel(
        IAuthenticationService authentication,
        IAppCoordinator coordinator,
        IValidator<LoginRequestDto> validator
    ) : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;
        _validator = validator;

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync);

        GoToRegisterCommand = ReactiveCommand.Create(() => _coordinator.ShowRegister()
        );
    }

    private ValidationResult ValidateDto()
    {
        var dto = new LoginRequestDto
        {
            Login = Model.Username,
            Password = Model.Password
        };
        return _validator.Validate(dto);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (!ValidateDto().IsValid)
        {
            ErrorMessage = "Wrong username or password";
            return;
        }

        var dto = new LoginRequestDto
        {
            Login = Model.Username,
            Password = Model.Password
        };

        try
        {
            await _authentication.LoginAsync(dto);
            _coordinator.ShowRegister();
        }
        catch (HttpRequestException httpEx) when ((int?)httpEx.StatusCode == 400)
        {
            ErrorMessage = "Wrong username or password";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}