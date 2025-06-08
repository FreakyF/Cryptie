namespace Cryptie.Common.Entities.LoginPolicy;

public class UserLoginAttempt
{
    public Guid Id { get; init; }
    public required User.User User { get; set; }
    public DateTime TimeStamp { get; set; }
}