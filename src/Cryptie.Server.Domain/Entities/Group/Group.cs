namespace Cryptie.Server.Domain.Features.Authentication.Entities.Group;

public class Group
{
    public Guid Id { get; init; }
    public required string Name { get; set; }

    public List<User.User> Users { get; } = [];
    public List<UserGroupPublicKey> Keys { get; } = [];
    public List<GroupMessage> Messages { get; } = [];
}