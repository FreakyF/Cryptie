namespace Cryptie.Server.Domain.Features.Authentication.Entities.SessionTokens;

public class TotpToken
{
    public Guid Id { get; init; }
    public User.User User { get; set; }
    public DateTime Until { get; set; }
}