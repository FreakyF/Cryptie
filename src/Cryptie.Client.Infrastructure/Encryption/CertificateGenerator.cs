using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Cryptie.Client.Infrastructure.Encryption;

public class CertificateGenerator
{
    private readonly CertificateRequest _request;

    public CertificateGenerator()
    {
        using var rsa = RSA.Create(2048);
        _request = new CertificateRequest("CN=Cryptie", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        _request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));
    }

    public X509Certificate2 GenerateCertificate()
    {
        return _request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
    }

    public static X509Certificate2 ExtractPrivateKey(X509Certificate2 certificate)
    {
        return X509CertificateLoader.LoadCertificate(certificate.Export(X509ContentType.Pfx));
    }

    public static X509Certificate2 ExtractPublicKey(X509Certificate2 certificate)
    {
        return X509CertificateLoader.LoadCertificate(certificate.Export(X509ContentType.Cert));
    }
}