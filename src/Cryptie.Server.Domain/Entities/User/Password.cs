namespace Cryptie.Server.Domain.Features.Authentication.Entities.User;

public class Password
{
    public Guid Id { get; init; }
    public string Secret { get; set; }
}