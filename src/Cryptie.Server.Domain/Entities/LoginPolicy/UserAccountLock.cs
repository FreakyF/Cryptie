namespace Cryptie.Server.Domain.Features.Authentication.Entities.LoginPolicy;

public class UserAccountLock
{
    public Guid Id { get; init; }
    public required User.User User { get; set; }
    public DateTime Until { get; set; }
}