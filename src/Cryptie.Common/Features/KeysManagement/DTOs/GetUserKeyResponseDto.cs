using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class GetUserKeyResponseDto
{
    public X509Certificate2 PublicKey { get; set; }
}