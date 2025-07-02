using System;
using System.Security.Cryptography.X509Certificates;
using Cryptie.Client.Encryption;
using Xunit;

namespace Cryptie.Client.Tests.Encryption
{
    public class MessageEncryptionTests
    {
        private readonly X509Certificate2 _publicCert;
        private readonly X509Certificate2 _privateCert;

        public MessageEncryptionTests()
        {
            var generator = new CertificateGenerator();
            var cert = generator.GenerateCertificate();
            _privateCert = cert;
            _publicCert = CertificateGenerator.ExtractPublicKey(cert);
        }

        [Fact]
        public void EncryptMessage_ReturnsBase64String()
        {
            var message = "test message";
            var encrypted = MessageEncryption.EncryptMessage(message, _publicCert);
            Assert.False(string.IsNullOrWhiteSpace(encrypted));
            var bytes = Convert.FromBase64String(encrypted);
            Assert.NotNull(bytes);
        }

        [Fact]
        public void DecryptMessage_ReturnsOriginalMessage()
        {
            var message = "test message";
            var encrypted = MessageEncryption.EncryptMessage(message, _publicCert);
            var decrypted = MessageEncryption.DecryptMessage(encrypted, _privateCert);
            Assert.Equal(message, decrypted);
        }

        [Fact]
        public void DecryptMessage_WithWrongKey_Throws()
        {
            var message = "test message";
            var encrypted = MessageEncryption.EncryptMessage(message, _publicCert);
            using var rsa = System.Security.Cryptography.RSA.Create(2048);
            var req = new CertificateRequest("CN=Other", rsa, System.Security.Cryptography.HashAlgorithmName.SHA256,
                System.Security.Cryptography.RSASignaturePadding.Pkcs1);
            var otherCert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));
            var wrongPrivate = new X509Certificate2(otherCert.Export(X509ContentType.Pfx));
            Assert.ThrowsAny<Exception>(() => MessageEncryption.DecryptMessage(encrypted, wrongPrivate));
        }

        [Fact]
        public void DecryptMessage_WithInvalidBase64_Throws()
        {
            Assert.Throws<FormatException>(() => MessageEncryption.DecryptMessage("not_base64", _privateCert));
        }
    }
}