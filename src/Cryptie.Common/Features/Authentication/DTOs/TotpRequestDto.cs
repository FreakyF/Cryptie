namespace Cryptie.Common.Features.Authentication.DTOs;

public class TotpRequestDto
{
    public required Guid TotpToken { get; set; }
    public required string Secret { get; set; }
}