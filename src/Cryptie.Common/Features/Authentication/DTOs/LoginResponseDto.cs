

namespace Cryptie.Common.Features.Authentication.DTOs;

public class LoginResponseDto
{
    public required Guid TotpToken { get; set; }
}