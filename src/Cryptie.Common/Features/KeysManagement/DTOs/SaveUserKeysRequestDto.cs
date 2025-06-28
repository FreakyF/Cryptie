using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class SaveUserKeysRequestDto
{
    public Guid userToken { get; set; }
    public string privateKey { get; set; }
    public string publicKey { get; set; }
}