namespace Cryptie.Server.Domain.Features.Authentication.DTOs;

public class RegisterRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
}