using System;
using ReactiveUI;

namespace Cryptie.Client.Features.Menu.State;

public class UserState : ReactiveObject, IUserState
{
    private string? _sessionToken;
    private Guid? _userId;
    private string? _username;

    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string? SessionToken
    {
        get => _sessionToken;
        set => this.RaiseAndSetIfChanged(ref _sessionToken, value);
    }

    public Guid? UserId
    {
        get => _userId;
        set => this.RaiseAndSetIfChanged(ref _userId, value);
    }
}