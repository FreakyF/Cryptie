using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Application;
using Cryptie.Client.Application.Features.Authentication.Services;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _shell;
    private string _username;
    private string _displayName;
    private string _password;
    private string _email;

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string DisplayName
    {
        get => _displayName;
        set => this.RaiseAndSetIfChanged(ref _displayName, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }
    
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToLoginCommand { get; }
    private readonly IAuthenticationService _authentication;

    public RegisterViewModel(IAuthenticationService authentication, MainWindowViewModel shell)
    {
        _authentication = authentication;
        _shell = shell;
        var canRegister = this.WhenAnyValue(
            x => x.Username,
            x => x.DisplayName,
            x => x.Email,
            x => x.Password,
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

        GoToLoginCommand = ReactiveCommand.Create(() =>
        {
            // zakładam, że Shell ma metodę ShowLogin()
            _shell.ShowLogin();
        });
    }
    
    private async Task RegisterAsync()
    {
        // ErrorMessage = string.Empty;

        var dto = new RegisterRequest
        {
            Login    = Username,
            DisplayName = DisplayName,
            Email       = Email,
            Password    = Password
        };

        try
        {
            await _authentication.RegisterAsync(dto);
            // po udanej rejestracji przejdź do logowania
            _shell.ShowLogin();
        }
        catch (Exception ex)
        {
            // Wyświetl błąd w UI
            // ErrorMessage = ex.Message;
        }
    }
}