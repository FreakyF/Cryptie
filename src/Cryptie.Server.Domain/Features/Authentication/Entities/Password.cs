namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class Password
{
    public Guid Id { get; init; }
    public string Secret { get; set; }
}