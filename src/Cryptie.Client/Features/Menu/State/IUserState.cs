using System;

namespace Cryptie.Client.Features.Menu.State;

public interface IUserState
{
    string? Username { get; set; }
    string? Login { get; set; }
    string? SessionToken { get; set; }
    string? PrivateKey { get; set; }
    Guid? UserId { get; set; }
}