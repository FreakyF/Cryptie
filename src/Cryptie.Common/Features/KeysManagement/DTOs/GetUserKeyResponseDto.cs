using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class GetUserKeyResponseDto
{
    public string PublicKey { get; set; } = string.Empty;
}