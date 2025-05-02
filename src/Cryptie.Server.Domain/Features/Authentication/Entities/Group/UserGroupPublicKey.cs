namespace Cryptie.Server.Domain.Features.Authentication.Entities.Group;

public class UserGroupPublicKey
{
    public Guid Id { get; init; }
    public User.User User { get; set; }
    public string Key { get; set; }
}