using System;
using System.Security.Cryptography;
using System.Text;

namespace Cryptie.Client.Encryption;

public class EncryptionKeyGenerator
{
    public string GenerateAesKey(int keySize = 256)
    {
        using var aes = Aes.Create();

        aes.KeySize = keySize;
        aes.GenerateKey();

        return Convert.ToBase64String(aes.Key);
    }
}