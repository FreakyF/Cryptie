using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Coordinators;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IAppCoordinator _coordinator;

    public LoginViewModel(
        IAuthenticationService authentication,
        IAppCoordinator coordinator
    ) : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;

        var canLogin = this.WhenAnyValue(
            x => x.Model.Username,
            x => x.Model.Password,
            (u, p) => !string.IsNullOrWhiteSpace(u) && !string.IsNullOrWhiteSpace(p)
        );

        LoginCommand = ReactiveCommand.CreateFromTask(LoginAsync, canLogin);
        GoToRegisterCommand = ReactiveCommand.Create(() => _coordinator.ShowRegister());
    }

    public LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    private async Task LoginAsync()
    {
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
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}