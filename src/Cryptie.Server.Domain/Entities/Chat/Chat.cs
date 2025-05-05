namespace Cryptie.Server.Domain.Features.Authentication.Entities.Chat;

public class Chat
{
    public Guid Id { get; init; }
    public string Name { get; set; }

    public User.User FirstUser { get; set; }
    public User.User SecondUser { get; set; }

    public string FirstUserPublicKey { get; set; }
    public string SecondUserPublicKey { get; set; }
}