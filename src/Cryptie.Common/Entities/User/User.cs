namespace Cryptie.Common.Entities.User;

public class User
{
    public Guid Id { get; init; }
    public required string Login { get; set; }

    public required string DisplayName { get; set; }
    public required string Email { get; set; }

    public required Password Password { get; set; }
    public required Totp Totp { get; set; }

    public List<Group.Group> Groups { get; } = [];

    public List<User> Friends { get; } = [];

    public string PrivateKey { get; set; }
}