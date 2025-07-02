using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities
{
    public class GroupEncryptionKeyTests
    {
        [Fact]
        public void GroupEncryptionKey_DefaultConstructor_InitializesProperties()
        {
            var key = new GroupEncryptionKey();
            Assert.Equal(Guid.Empty, key.Id);
            Assert.Equal(Guid.Empty, key.GroupId);
            Assert.Equal(Guid.Empty, key.UserId);
            Assert.Null(key.AesKey); // null! pozwala na null, dopóki nie zostanie przypisane
            Assert.Null(key.Group);  // null! pozwala na null, dopóki nie zostanie przypisane
        }

        [Fact]
        public void GroupEncryptionKey_SetAndGetProperties_WorksCorrectly()
        {
            var id = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var aesKey = "testkey";
            var group = new Group();

            var key = new GroupEncryptionKey
            {
                Id = id,
                GroupId = groupId,
                UserId = userId,
                AesKey = aesKey,
                Group = group
            };

            Assert.Equal(id, key.Id);
            Assert.Equal(groupId, key.GroupId);
            Assert.Equal(userId, key.UserId);
            Assert.Equal(aesKey, key.AesKey);
            Assert.Equal(group, key.Group);
        }

        [Fact]
        public void GroupEncryptionKey_UserProperty_CanBeSetAndRead()
        {
            var user = new User { Id = Guid.NewGuid(), Login = "test", DisplayName = "Test", Email = "test@test.com", PrivateKey = "priv", PublicKey = "pub" };
            var key = new GroupEncryptionKey { User = user };
            Assert.Equal(user, key.User);
        }

        [Fact]
        public void GroupEncryptionKey_SetAndGetAllProperties_WorksCorrectly()
        {
            var id = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var aesKey = "testkey";
            var group = new Group();
            var user = new User();

            var key = new GroupEncryptionKey
            {
                Id = id,
                GroupId = groupId,
                UserId = userId,
                AesKey = aesKey,
                Group = group,
                User = user
            };

            Assert.Equal(id, key.Id);
            Assert.Equal(groupId, key.GroupId);
            Assert.Equal(userId, key.UserId);
            Assert.Equal(aesKey, key.AesKey);
            Assert.Equal(group, key.Group);
            Assert.Equal(user, key.User);
        }
    }
}
