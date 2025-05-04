using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Application;
using Cryptie.Client.Application.Features.Authentication.Services;
using Cryptie.Client.Desktop.Models;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _shell;
    private readonly IAuthenticationService _authentication;

    private LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    public LoginViewModel(IAuthenticationService authentication, MainWindowViewModel shell)
    {
        _authentication = authentication;
        _shell = shell;

        var canRegister = this.WhenAnyValue(
            x => x.Model.Username,
            x => x.Model.Password,
            (u, d) =>
                !string.IsNullOrWhiteSpace(u) &&
                !string.IsNullOrWhiteSpace(d)
        );

        LoginCommand = ReactiveCommand.CreateFromTask(
            execute: LoginAsync,
            canExecute: canRegister
        );
        GoToRegisterCommand = ReactiveCommand.Create(() => { _shell.ShowRegister(); });
    }

    private async Task LoginAsync()
    {
        // ErrorMessage = string.Empty;

        var dto = new LoginRequest
        {
            Login = Model.Username,
            Password = Model.Password
        };

        try
        {
            await _authentication.LoginAsync(dto);

            _shell.ShowRegister();
        }
        catch (Exception ex)
        {
            // Wyświetl błąd w UI
            // ErrorMessage = ex.Message;
        }
    }
}