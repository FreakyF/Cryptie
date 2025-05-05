namespace Cryptie.Server.Domain.Features.Authentication.Entities.SessionTokens;

public class UserToken
{
    public Guid Id { get; init; }
    public User.User? User { get; set; }
}