using System;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.KeysManagement.DTOs;

public class SaveUserKeysRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var userToken = Guid.NewGuid();
        var privateKey = "private-key-value";
        var publicKey = "public-key-value";
        // Act
        var dto = new SaveUserKeysRequestDto
        {
            userToken = userToken,
            privateKey = privateKey,
            publicKey = publicKey
        };
        // Assert
        Assert.Equal(userToken, dto.userToken);
        Assert.Equal(privateKey, dto.privateKey);
        Assert.Equal(publicKey, dto.publicKey);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new SaveUserKeysRequestDto();
        // Assert
        Assert.Equal(Guid.Empty, dto.userToken);
        Assert.Null(dto.privateKey);
        Assert.Null(dto.publicKey);
    }
}

