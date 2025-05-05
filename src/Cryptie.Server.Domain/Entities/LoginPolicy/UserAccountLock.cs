namespace Cryptie.Server.Domain.Features.Authentication.Entities.LoginPolicy;

public class UserAccountLock
{
    public Guid Id { get; init; }
    public User.User User { get; set; }
    public DateTime Until { get; set; }
}