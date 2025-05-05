namespace Cryptie.Server.Domain.Features.Authentication.Entities.Chat;

public class ChatMessages
{
    public Guid Id { get; init; }
    public Guid ReferenceId { get; set; }

    public User.User FromUser { get; set; }
    public User.User ToUser { get; set; }

    public string Message { get; set; }
    public DateTime DateTime { get; set; }
}