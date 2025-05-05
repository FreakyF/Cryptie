namespace Cryptie.Common.Features.Authentication.DTOs;

public class RegisterResponseDto
{
    public required string Secret { get; init; }
    public required Guid TotpToken { get; set; }
}