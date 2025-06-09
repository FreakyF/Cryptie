namespace Cryptie.Common.Entities.Honeypot;

public class UserAccountHoneypotLock
{
    public Guid Id { get; init; }
    public required string User { get; set; }
    public DateTime Until { get; set; }
}