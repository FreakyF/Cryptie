using Cryptie.Client.Encryption;

namespace Cryptie.Client.Tests.Encryption
{
    public class EncryptionKeyGeneratorTests
    {
        [Theory]
        [InlineData(128)]
        [InlineData(192)]
        [InlineData(256)]
        public void GenerateAesKey_ReturnsKeyOfCorrectSize(int keySize)
        {
            var key = EncryptionKeyGenerator.GenerateAesKey(keySize);

            Assert.NotNull(key);
            Assert.Equal(keySize / 8, key.Length);
        }

        [Fact]
        public void GenerateAesKey_KeysAreRandom()
        {
            var key1 = EncryptionKeyGenerator.GenerateAesKey();
            var key2 = EncryptionKeyGenerator.GenerateAesKey();

            Assert.NotEqual(Convert.ToBase64String(key1), Convert.ToBase64String(key2));
        }
    }
}

