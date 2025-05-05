namespace Cryptie.Server.Domain.Features.Authentication.Entities.Group;

public class UserGroupPublicKey
{
    public Guid Id { get; init; }
    public required User.User User { get; set; }
    public required string Key { get; set; }
}