using System;
using System.Collections.Generic;
using Cryptie.Common.Features.UserManagement.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.UserManagement.DTOs;

public class AddFriendRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var sessionToken = Guid.NewGuid();
        var friend = "TestFriend";
        var keys = new Dictionary<Guid, string> { { Guid.NewGuid(), "key1" } };

        // Act
        var dto = new AddFriendRequestDto
        {
            SessionToken = sessionToken,
            Friend = friend,
            EncryptionKeys = keys
        };

        // Assert
        Assert.Equal(sessionToken, dto.SessionToken);
        Assert.Equal(friend, dto.Friend);
        Assert.Equal(keys, dto.EncryptionKeys);
    }

    [Fact]
    public void EncryptionKeys_Defaults_To_Empty_Dictionary()
    {
        // Act
        var dto = new AddFriendRequestDto();
        // Assert
        Assert.NotNull(dto.EncryptionKeys);
        Assert.Empty(dto.EncryptionKeys);
    }

    [Fact]
    public void Can_Add_EncryptionKey()
    {
        // Arrange
        var dto = new AddFriendRequestDto();
        var keyId = Guid.NewGuid();
        var keyValue = "test-key";
        // Act
        dto.EncryptionKeys.Add(keyId, keyValue);
        // Assert
        Assert.True(dto.EncryptionKeys.ContainsKey(keyId));
        Assert.Equal(keyValue, dto.EncryptionKeys[keyId]);
    }
}
