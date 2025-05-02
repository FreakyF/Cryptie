using System.Reactive;
using ReactiveUI;

namespace Cryptie.Client.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _shell;
    private string _username;
    private string _password;
              
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToRegisterCommand { get; }

    public LoginViewModel(MainWindowViewModel shell)
    {
        _shell = shell;
        LoginCommand = ReactiveCommand.Create(() => { });
        GoToRegisterCommand = ReactiveCommand.Create(() => { _shell.ShowRegister(); });
    }
}