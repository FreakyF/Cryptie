namespace Cryptie.Common.Features.Authentication.DTOs;

public class LoginRequestDto
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}