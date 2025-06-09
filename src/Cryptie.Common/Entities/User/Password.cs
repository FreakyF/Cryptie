namespace Cryptie.Common.Entities.User;

public class Password
{
    public Guid Id { get; init; }
    public required string Secret { get; set; }
}