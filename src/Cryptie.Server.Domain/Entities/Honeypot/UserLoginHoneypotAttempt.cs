namespace Cryptie.Server.Domain.Features.Authentication.Entities.Honeypot;

public class UserLoginHoneypotAttempt
{
    public Guid Id { get; init; }
    public string User { get; set; }
    public DateTime TimeStamp { get; set; }
}