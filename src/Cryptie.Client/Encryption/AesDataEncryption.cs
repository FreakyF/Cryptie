using System;
using System.IO;
using System.Security.Cryptography;

namespace Cryptie.Client.Encryption;

public static class AesDataEncryption
{
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