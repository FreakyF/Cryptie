using System;
using Cryptie.Common.Features.GroupManagement;
using Xunit;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class AddUserToGroupRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var sessionToken = Guid.NewGuid();
        var groupGuid = Guid.NewGuid();
        var userToAdd = Guid.NewGuid();
        var encryptionKey = "test-key";

        // Act
        var dto = new AddUserToGroupRequestDto
        {
            SessionToken = sessionToken,
            GroupGuid = groupGuid,
            UserToAdd = userToAdd,
            EncryptionKey = encryptionKey
        };

        // Assert
        Assert.Equal(sessionToken, dto.SessionToken);
        Assert.Equal(groupGuid, dto.GroupGuid);
        Assert.Equal(userToAdd, dto.UserToAdd);
        Assert.Equal(encryptionKey, dto.EncryptionKey);
    }

    [Fact]
    public void EncryptionKey_Can_Be_Null()
    {
        // Act
        var dto = new AddUserToGroupRequestDto { EncryptionKey = null };
        // Assert
        Assert.Null(dto.EncryptionKey);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new AddUserToGroupRequestDto();
        // Assert
        Assert.Equal(default(Guid), dto.SessionToken);
        Assert.Equal(default(Guid), dto.GroupGuid);
        Assert.Equal(default(Guid), dto.UserToAdd);
        Assert.Null(dto.EncryptionKey);
    }
}

