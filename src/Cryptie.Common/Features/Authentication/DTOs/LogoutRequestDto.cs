

namespace Cryptie.Common.Features.Authentication.DTOs;

public class LogoutRequestDto
{
    public required Guid Token { get; set; }
}