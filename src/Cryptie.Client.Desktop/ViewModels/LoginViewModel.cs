using System;
using System.Reactive;
using System.Threading.Tasks;
using Cryptie.Client.Desktop.Models;
using Cryptie.Client.Domain.Features.Authentication.Services;
using Cryptie.Common.Features.Authentication.DTOs;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly IAuthenticationService _authentication;
    private readonly MainWindowViewModel _shell;

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
            LoginAsync,
            canRegister
        );
        GoToRegisterCommand = ReactiveCommand.Create(() => { _shell.ShowRegister(); });
    }

    internal LoginModel Model { get; } = new();
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    public string UrlPathSegment { get; } = Guid.NewGuid().ToString()[..5];
    public IScreen HostScreen { get; }

    private async Task LoginAsync()
    {
        // ErrorMessage = string.Empty;

        var dto = new LoginRequestDto
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