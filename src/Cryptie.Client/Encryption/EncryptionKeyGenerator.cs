using System.Security.Cryptography;

namespace Cryptie.Client.Encryption;

public static class EncryptionKeyGenerator
{
    public static byte[] GenerateAesKey(int keySize = 256)
    {
        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.GenerateKey();
        return aes.Key;
    }
}