using ReactiveUI;

namespace Cryptie.Client.Features.Menu.State;

public class UserState : ReactiveObject, IUserState
{
    private string? _username;

    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
}