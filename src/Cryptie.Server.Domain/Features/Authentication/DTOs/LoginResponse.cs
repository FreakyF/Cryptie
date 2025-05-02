namespace Cryptie.Server.Domain.Features.Authentication.DTOs;

public class LoginResponse
{
    public required Guid TotpToken { get; set; }
}