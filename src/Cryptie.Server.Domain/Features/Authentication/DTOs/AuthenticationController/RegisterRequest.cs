namespace Cryptie.Server.Domain.Features.Authentication.DTOs;

public class RegisterRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
}