namespace Cryptie.Server.Domain.Features.Authentication.Entities.Chat;

public class Chat
{
    public Guid Id { get; init; }
    public required string Name { get; set; }

    public required User.User FirstUser { get; set; }
    public required User.User SecondUser { get; set; }

    public required string FirstUserPublicKey { get; set; }
    public required string SecondUserPublicKey { get; set; }
}