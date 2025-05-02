namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class TotpToken
{
    public Guid Id { get; init; }
    public User User { get; set; }
    public DateTime Until { get; set; }
}