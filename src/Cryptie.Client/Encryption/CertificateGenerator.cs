using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Client.Encryption;

public static class CertificateGenerator
{
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

    public static X509Certificate2 ExtractPublicKey(X509Certificate2 certificate)
    {
        return X509CertificateLoader.LoadCertificate(certificate.Export(X509ContentType.Cert));
    }
}