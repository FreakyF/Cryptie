namespace Cryptie.Common.Entities.User;

public class Totp
{
    public Guid Id { get; init; }
    public required byte[] Secret { get; init; }
}