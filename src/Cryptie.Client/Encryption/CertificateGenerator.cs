using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Client.Encryption;

public static class CertificateGenerator
{
    /// <summary>
    ///     Generates a self-signed certificate that can be used for RSA encryption.
    /// </summary>
    /// <returns>A new <see cref="X509Certificate2" /> containing both private and public keys.</returns>
    public static X509Certificate2 GenerateCertificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=Cryptie",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions
            .Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

        return request.CreateSelfSigned(
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddYears(1));
    }

    /// <summary>
    ///     Extracts the public portion of the provided certificate.
    /// </summary>
    /// <param name="certificate">Certificate containing the key pair.</param>
    /// <returns>A certificate containing only the public key.</returns>
    public static X509Certificate2 ExtractPublicKey(X509Certificate2 certificate)
    {
        return X509CertificateLoader.LoadCertificate(certificate.Export(X509ContentType.Cert));
    }
}