using System.Reactive;
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


    public RegisterViewModel(MainWindowViewModel shell)
    {
        _shell = shell;
        RegisterCommand = ReactiveCommand.Create(() => { });
        GoToLoginCommand = ReactiveCommand.Create(() => { _shell.ShowLogin(); });
    }
}