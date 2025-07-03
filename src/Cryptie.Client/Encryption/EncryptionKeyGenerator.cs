using System.Security.Cryptography;

namespace Cryptie.Client.Encryption;

public static class EncryptionKeyGenerator
{
    /// <summary>
    ///     Generates a new AES key with the specified key size.
    /// </summary>
    /// <param name="keySize">Size of the key in bits. Defaults to 256.</param>
    /// <returns>Byte array containing the generated key.</returns>
    public static byte[] GenerateAesKey(int keySize = 256)
    {
        using var aes = Aes.Create();
        aes.KeySize = keySize;
        aes.GenerateKey();
        return aes.Key;
    }
}