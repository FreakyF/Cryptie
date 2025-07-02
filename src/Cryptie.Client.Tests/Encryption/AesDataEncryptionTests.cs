using System.Security.Cryptography;
using Cryptie.Client.Encryption;

namespace Cryptie.Client.Tests.Encryption
{
    public class AesDataEncryptionTests
    {
        private static string GenerateAesKey()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        [Fact]
        public void EncryptDataAES_And_DecryptDataAES_ShouldReturnOriginalData()
        {
            var key = GenerateAesKey();
            var originalData = "Test data for encryption!";


            var encrypted = AesDataEncryption.Encrypt(originalData, key);
            var decrypted = AesDataEncryption.Decrypt(encrypted, key);


            Assert.NotNull(encrypted);
            Assert.NotEqual(originalData, encrypted);
            Assert.Equal(originalData, decrypted);
            Assert.True(originalData.SequenceEqual(decrypted), "Dane po odszyfrowaniu nie są identyczne z oryginałem");
        }

        [Fact]
        public void DecryptDataAES_WithWrongKey_ShouldThrow()
        {
            var key = GenerateAesKey();
            var wrongKey = GenerateAesKey();
            var originalData = "Sensitive data";
            var encrypted = AesDataEncryption.Encrypt(originalData, key);


            Assert.ThrowsAny<Exception>(() => AesDataEncryption.Decrypt(encrypted, wrongKey));
        }

        [Fact]
        public void EncryptDataAES_WithNullData_ShouldReturnEncrypted()
        {
            var key = GenerateAesKey();
            string? data = null;


            var encrypted = AesDataEncryption.Encrypt(data, key);
            var decrypted = AesDataEncryption.Decrypt(encrypted, key);


            Assert.Equal("", decrypted);
        }
    }
}