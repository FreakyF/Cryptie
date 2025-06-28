using System;

namespace Cryptie.Client.Encryption;

public class DataEncryption
{
    public string EncryptDataAES(string data, string key)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new System.IO.MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor,
                   System.Security.Cryptography.CryptoStreamMode.Write))
        {
            using var sw = new System.IO.StreamWriter(cs);
            sw.Write(data);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string DecryptDataAES(string encryptedData, string key)
    {
        var fullCipher = Convert.FromBase64String(encryptedData);
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = Convert.FromBase64String(key);

        var iv = new byte[aes.IV.Length];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new System.IO.MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor,
            System.Security.Cryptography.CryptoStreamMode.Read);
        using var sr = new System.IO.StreamReader(cs);

        return sr.ReadToEnd();
    }
}