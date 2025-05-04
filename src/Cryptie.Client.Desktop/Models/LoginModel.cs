using ReactiveUI;

namespace Cryptie.Client.Desktop.Models;

public class LoginModel : ReactiveObject
{
    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
}