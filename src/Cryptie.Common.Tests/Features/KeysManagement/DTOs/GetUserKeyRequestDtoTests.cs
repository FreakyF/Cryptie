using System;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.KeysManagement.DTOs;

public class GetUserKeyRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_UserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new GetUserKeyRequestDto();
        // Act
        dto.UserId = userId;
        // Assert
        Assert.Equal(userId, dto.UserId);
    }

    [Fact]
    public void Default_UserId_Is_Empty()
    {
        // Act
        var dto = new GetUserKeyRequestDto();
        // Assert
        Assert.Equal(Guid.Empty, dto.UserId);
    }
}

