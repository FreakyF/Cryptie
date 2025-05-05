namespace Cryptie.Server.Domain.Features.Authentication.Entities.Group;

public class GroupMessage
{
    public Guid Id { get; init; }
    public Guid ReferenceId { get; set; }

    public required User.User FromUser { get; set; }
    public required User.User ToUser { get; set; }

    public required string Message { get; set; }
    public DateTime DateTime { get; set; }
}