using System;
using Cryptie.Client.Encryption;
using Xunit;

namespace Cryptie.Client.Tests.Encryption
{
    public class DataEncryptionTests
    {
        private readonly DataEncryption _dataEncryption = new DataEncryption();

        private static string GenerateAesKey()
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        [Fact]
        public void EncryptDataAES_And_DecryptDataAES_ShouldReturnOriginalData()
        {
            var key = GenerateAesKey();
            var originalData = "Test data for encryption!";


            var encrypted = _dataEncryption.EncryptDataAES(originalData, key);
            var decrypted = _dataEncryption.DecryptDataAES(encrypted, key);


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
            var encrypted = _dataEncryption.EncryptDataAES(originalData, key);


            Assert.ThrowsAny<Exception>(() => _dataEncryption.DecryptDataAES(encrypted, wrongKey));
        }

        [Fact]
        public void EncryptDataAES_WithNullData_ShouldReturnEncrypted()
        {
            var key = GenerateAesKey();
            string? data = null;


            var encrypted = _dataEncryption.EncryptDataAES(data, key);
            var decrypted = _dataEncryption.DecryptDataAES(encrypted, key);


            Assert.Equal("", decrypted);
        }
    }
}