namespace Cryptie.Server.Domain.Features.Authentication.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    public Password Password { get; set; }
    public Totp Totp { get; set; }
}