using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Composition.Factories;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class RegisterViewModel : RoutableViewModelBase
{
    private readonly IAuthenticationService _authentication;
    private readonly IAppCoordinator _coordinator;

    public RegisterModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

    public RegisterViewModel(
        IAuthenticationService authentication,
        IAppCoordinator coordinator
    ) : base(coordinator)
    {
        _authentication = authentication;
        _coordinator = coordinator;

        var canRegister = this.WhenAnyValue(
            x => x.Model.Username,
            x => x.Model.DisplayName,
            x => x.Model.Email,
            x => x.Model.Password,
            (u, d, e, p) =>
                !string.IsNullOrWhiteSpace(u) &&
                !string.IsNullOrWhiteSpace(d) &&
                !string.IsNullOrWhiteSpace(e) &&
                !string.IsNullOrWhiteSpace(p)
        );

        RegisterCommand = ReactiveCommand.CreateFromTask(RegisterAsync, canRegister);
        GoToLoginCommand = ReactiveCommand.Create(() => _coordinator.ShowLogin());
    }

    private async Task RegisterAsync()
    {
        var dto = new RegisterRequestDto
        {
            Login = Model.Username,
            DisplayName = Model.DisplayName,
            Email = Model.Email,
            Password = Model.Password
        };

        try
        {
            await _authentication.RegisterAsync(dto);
            _coordinator.ShowLogin();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}