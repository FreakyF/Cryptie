using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptie.Client.Encryption;

public static class RsaDataEncryption
{
    public static string Encrypt(string message, X509Certificate2 publicKey)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var contentInfo = new ContentInfo(messageBytes);
        var envelopedCms = new EnvelopedCms(contentInfo);
        var recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, publicKey);

        envelopedCms.Encrypt(recipient);

        return Convert.ToBase64String(envelopedCms.Encode());
    }

    public static string Decrypt(string message, X509Certificate2 privateKey)
    {
        var messageBytes = Convert.FromBase64String(message);
        var envelopedCms = new EnvelopedCms();

        envelopedCms.Decode(messageBytes);
        envelopedCms.Decrypt(new X509Certificate2Collection(privateKey));

        return Encoding.UTF8.GetString(envelopedCms.ContentInfo.Content);
    }

    public static X509Certificate2 LoadCertificateFromBase64(string base64, X509ContentType contentType,
        string? password = null)
    {
        var bytes = Convert.FromBase64String(base64);
        if (contentType == X509ContentType.Pfx && !string.IsNullOrEmpty(password))
            return new X509Certificate2(bytes, password, X509KeyStorageFlags.Exportable);
        
        return new X509Certificate2(bytes);
    }
}