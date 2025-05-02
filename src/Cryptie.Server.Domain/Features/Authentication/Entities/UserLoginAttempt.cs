namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class UserLoginAttempt
{
    public Guid Id { get; init; }
    public User User { get; set; }
    public DateTime TimeStamp { get; set; }
}