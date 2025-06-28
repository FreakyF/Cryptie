using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class SaveUserKeysRequestDto
{
    public Guid userToken { get; set; }
    public X509Certificate2 privateKey { get; set; }
    public X509Certificate2 publicKey { get; set; }
}