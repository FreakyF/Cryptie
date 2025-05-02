namespace Cryptie.Server.Domain.Features.Authentication.DTOs;

public class LogoutRequest
{
    public required Guid Token { get; set; }
}