namespace Cryptie.Server.Domain.Features.Authentication.Entities.Honeypot;

public class UserAccountHoneypotLock
{
    public Guid Id { get; init; }
    public string User { get; set; }
    public DateTime Until { get; set; }
}