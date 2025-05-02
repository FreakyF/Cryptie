namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class UserLoginHoneypotAttempt
{
    public Guid Id { get; init; }
    public string User { get; set; }
    public DateTime TimeStamp { get; set; }
}