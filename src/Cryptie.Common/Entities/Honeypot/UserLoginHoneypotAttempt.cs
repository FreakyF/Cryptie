

namespace Cryptie.Common.Entities.Honeypot;

public class UserLoginHoneypotAttempt
{
    public Guid Id { get; init; }
    public required string User { get; set; }
    public DateTime TimeStamp { get; set; }
}