using System;
using System.Collections.Generic;
using Cryptie.Common.Features.GroupManagement;
using Xunit;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class CreateGroupFromPrivateChatRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var sessionToken = Guid.NewGuid();
        var privateChatId = Guid.NewGuid();
        var newMember = "TestUser";
        var encryptionKeys = new Dictionary<Guid, string> { { Guid.NewGuid(), "key1" } };

        // Act
        var dto = new CreateGroupFromPrivateChatRequestDto
        {
            SessionToken = sessionToken,
            PrivateChatId = privateChatId,
            NewMember = newMember,
            EncryptionKeys = encryptionKeys
        };

        // Assert
        Assert.Equal(sessionToken, dto.SessionToken);
        Assert.Equal(privateChatId, dto.PrivateChatId);
        Assert.Equal(newMember, dto.NewMember);
        Assert.Equal(encryptionKeys, dto.EncryptionKeys);
    }

    [Fact]
    public void EncryptionKeys_Defaults_To_Empty_Dictionary()
    {
        // Act
        var dto = new CreateGroupFromPrivateChatRequestDto();
        // Assert
        Assert.NotNull(dto.EncryptionKeys);
        Assert.Empty(dto.EncryptionKeys);
    }

    [Fact]
    public void Can_Add_EncryptionKey()
    {
        // Arrange
        var dto = new CreateGroupFromPrivateChatRequestDto();
        var keyId = Guid.NewGuid();
        var keyValue = "test-key";
        // Act
        dto.EncryptionKeys.Add(keyId, keyValue);
        // Assert
        Assert.True(dto.EncryptionKeys.ContainsKey(keyId));
        Assert.Equal(keyValue, dto.EncryptionKeys[keyId]);
    }
}
