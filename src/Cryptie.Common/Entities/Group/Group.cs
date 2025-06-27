namespace Cryptie.Common.Entities.Group;

public class Group
{
    public Guid Id { get; init; }
    public required string Name { get; set; }

    public List<User.User> Users { get; } = [];
    public List<GroupMessage> Messages { get; } = [];
}