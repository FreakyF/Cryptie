namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class Totp
{
    public Guid Id { get; init; }
    public required byte[] Secret { get; init; }
}