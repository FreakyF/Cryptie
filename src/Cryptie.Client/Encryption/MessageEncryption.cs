using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptie.Client.Encryption;

public static class MessageEncryption
{
    public static string EncryptMessage(string message, X509Certificate2 publicKey)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var contentInfo = new ContentInfo(messageBytes);
        var envelopedCms = new EnvelopedCms(contentInfo);
        var recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, publicKey);

        envelopedCms.Encrypt(recipient);

        return Convert.ToBase64String(envelopedCms.Encode());
    }

    public static string DecryptMessage(string message, X509Certificate2 privateKey)
    {
        var messageBytes = Convert.FromBase64String(message);
        var envelopedCms = new EnvelopedCms();

        envelopedCms.Decode(messageBytes);
        envelopedCms.Decrypt(new X509Certificate2Collection(privateKey));

        return Encoding.UTF8.GetString(envelopedCms.ContentInfo.Content);
    }
}