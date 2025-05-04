using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Application;
using Cryptie.Client.Application.Features.Authentication.Services;
using Cryptie.Client.Desktop.Models;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _shell;
    private readonly IAuthenticationService _authentication;
    private RegisterModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }

    public RegisterViewModel(IAuthenticationService authentication, MainWindowViewModel shell)
    {
        _authentication = authentication;
        _shell = shell;
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

        RegisterCommand = ReactiveCommand.CreateFromTask(
            execute: RegisterAsync,
            canExecute: canRegister
        );

        GoToLoginCommand = ReactiveCommand.Create(() => { _shell.ShowLogin(); });
    }

    private async Task RegisterAsync()
    {
        // ErrorMessage = string.Empty;

        var dto = new RegisterRequest
        {
            Login = Model.Username,
            DisplayName = Model.DisplayName,
            Email = Model.Email,
            Password = Model.Password
        };

        try
        {
            await _authentication.RegisterAsync(dto);

            _shell.ShowLogin();
        }
        catch (Exception ex)
        {
            // Wyświetl błąd w UI
            // ErrorMessage = ex.Message;
        }
    }
}