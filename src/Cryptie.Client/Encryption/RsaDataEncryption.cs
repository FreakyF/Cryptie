using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cryptie.Client.Encryption;

public static class RsaDataEncryption
{
    /// <summary>
    ///     Encrypts the provided <paramref name="message" /> with the recipient's public key.
    /// </summary>
    /// <param name="message">Plain text message to encrypt.</param>
    /// <param name="publicKey">Recipient's RSA public certificate.</param>
    /// <returns>Base64 encoded encrypted payload.</returns>
    public static string Encrypt(string message, X509Certificate2 publicKey)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var contentInfo = new ContentInfo(messageBytes);
        var envelopedCms = new EnvelopedCms(contentInfo);
        var recipient = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, publicKey);

        envelopedCms.Encrypt(recipient);

        return Convert.ToBase64String(envelopedCms.Encode());
    }

    /// <summary>
    ///     Decrypts an encrypted CMS message with the given private key.
    /// </summary>
    /// <param name="message">Base64 encoded encrypted data.</param>
    /// <param name="privateKey">Certificate containing the private key used for decryption.</param>
    /// <returns>The decrypted plain text message.</returns>
    public static string Decrypt(string message, X509Certificate2 privateKey)
    {
        var messageBytes = Convert.FromBase64String(message);
        var envelopedCms = new EnvelopedCms();

        envelopedCms.Decode(messageBytes);
        envelopedCms.Decrypt(new X509Certificate2Collection(privateKey));

        return Encoding.UTF8.GetString(envelopedCms.ContentInfo.Content);
    }

    /// <summary>
    ///     Loads an <see cref="X509Certificate2" /> instance from a Base64 encoded string.
    /// </summary>
    /// <param name="base64">The Base64 encoded certificate.</param>
    /// <param name="contentType">The format of the encoded certificate.</param>
    /// <param name="password">Optional password for PFX certificates.</param>
    /// <returns>The decoded certificate.</returns>
    public static X509Certificate2 LoadCertificateFromBase64(string base64, X509ContentType contentType,
        string? password = null)
    {
        var bytes = Convert.FromBase64String(base64);

        return contentType == X509ContentType.Pfx
            ? X509CertificateLoader.LoadPkcs12(bytes, password, X509KeyStorageFlags.Exportable)
            : X509CertificateLoader.LoadCertificate(bytes);
    }
}