namespace Cryptie.Server.Domain.Features.Authentication.Entities.LoginPolicy;

public class UserLoginAttempt
{
    public Guid Id { get; init; }
    public User.User User { get; set; }
    public DateTime TimeStamp { get; set; }
}