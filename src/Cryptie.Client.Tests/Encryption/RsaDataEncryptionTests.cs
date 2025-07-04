using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Cryptie.Client.Encryption;

namespace Cryptie.Client.Tests.Encryption
{
    public class RsaDataEncryptionTests
    {
        private readonly X509Certificate2 _privateCert;
        private readonly X509Certificate2 _publicCert;

        public RsaDataEncryptionTests()
        {
            var cert = CertificateGenerator.GenerateCertificate();
            _privateCert = cert;
            _publicCert = CertificateGenerator.ExtractPublicKey(cert);
        }

        [Fact]
        public void EncryptMessage_ReturnsBase64String()
        {
            var message = "test message";
            var encrypted = RsaDataEncryption.Encrypt(message, _publicCert);
            Assert.False(string.IsNullOrWhiteSpace(encrypted));
            var bytes = Convert.FromBase64String(encrypted);
            Assert.NotNull(bytes);
        }

        [Fact]
        public void DecryptMessage_ReturnsOriginalMessage()
        {
            var message = "test message";
            var encrypted = RsaDataEncryption.Encrypt(message, _publicCert);
            var decrypted = RsaDataEncryption.Decrypt(encrypted, _privateCert);
            Assert.Equal(message, decrypted);
        }

        [Fact]
        public void DecryptMessage_WithWrongKey_Throws()
        {
            var message = "test message";
            var encrypted = RsaDataEncryption.Encrypt(message, _publicCert);
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest("CN=Other", rsa, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            var otherCert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));
            var wrongPrivate = new X509Certificate2(otherCert.Export(X509ContentType.Pfx));
            Assert.ThrowsAny<Exception>(() => RsaDataEncryption.Decrypt(encrypted, wrongPrivate));
        }

        [Fact]
        public void DecryptMessage_WithInvalidBase64_Throws()
        {
            Assert.Throws<FormatException>(() => RsaDataEncryption.Decrypt("not_base64", _privateCert));
        }
    }
}