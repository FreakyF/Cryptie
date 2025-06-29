namespace Cryptie.Client.Features.Menu.State;

public interface IUserState
{
    string? Username { get; set; }
    string? SessionToken { get; set; }
}