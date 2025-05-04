using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Application;
using Cryptie.Client.Application.Features.Authentication.Services;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Desktop.Services;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authentication;
    private RegisterModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

    public RegisterViewModel(IAuthenticationService authentication, INavigationService navigationService)
    {
        _authentication = authentication;
        _navigationService = navigationService;
        var canRegister = this.WhenAnyValue(
            x => x.Model.Login,
            x => x.Model.DisplayName,
            x => x.Model.Email,
            x => x.Model.Password,
            (u, d, e, p) =>
                !string.IsNullOrWhiteSpace(u) &&
                !string.IsNullOrWhiteSpace(d) &&
                !string.IsNullOrWhiteSpace(e) &&
                !string.IsNullOrWhiteSpace(p)
        );

        RegisterCommand = ReactiveCommand.CreateFromTask(
            execute: RegisterAsync,
            canExecute: canRegister
        );

        GoToLoginCommand = ReactiveCommand.Create(() => { _navigationService.NavigateToLogin(); });
    }

    private async Task RegisterAsync()
    {
        // ErrorMessage = string.Empty;

        var dto = new RegisterRequest
        {
            Login = Model.Login,
            DisplayName = Model.DisplayName,
            Email = Model.Email,
            Password = Model.Password
        };

        try
        {
            await _authentication.RegisterAsync(dto);

            _navigationService.NavigateToLogin();
        }
        catch (Exception ex)
        {
            // Wyświetl błąd w UI
            // ErrorMessage = ex.Message;
        }
    }
}