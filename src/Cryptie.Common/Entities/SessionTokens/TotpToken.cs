namespace Cryptie.Common.Entities.SessionTokens;

public class TotpToken
{
    public Guid Id { get; init; }
    public required User.User User { get; set; }
    public DateTime Until { get; set; }
}