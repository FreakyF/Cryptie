using System;
using System.IO;
using System.Security.Cryptography;

namespace Cryptie.Client.Encryption;

public static class AesDataEncryption
{
    /// <summary>
    ///     Encrypts the provided string with the given AES key.
    /// </summary>
    /// <param name="data">Plain text to encrypt.</param>
    /// <param name="key">Base64 encoded AES key.</param>
    /// <returns>Base64 encoded cipher text containing the IV and encrypted payload.</returns>
    public static string Encrypt(string data, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor,
                   CryptoStreamMode.Write))
        {
            using var sw = new StreamWriter(cs);
            sw.Write(data);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    ///     Decrypts the given cipher text using the supplied AES key.
    /// </summary>
    /// <param name="encryptedData">Base64 encoded cipher that contains IV and encrypted data.</param>
    /// <param name="key">Base64 encoded AES key.</param>
    /// <returns>The decrypted plain text.</returns>
    public static string Decrypt(string encryptedData, string key)
    {
        var fullCipher = Convert.FromBase64String(encryptedData);
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);

        var iv = new byte[aes.IV.Length];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using var cs = new CryptoStream(ms, decryptor,
            CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}