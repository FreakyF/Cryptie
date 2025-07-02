using System;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetGroupMessagesRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var userToken = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        // Act
        var dto = new GetGroupMessagesRequestDto
        {
            UserToken = userToken,
            GroupId = groupId
        };
        // Assert
        Assert.Equal(userToken, dto.UserToken);
        Assert.Equal(groupId, dto.GroupId);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new GetGroupMessagesRequestDto();
        // Assert
        Assert.Equal(default(Guid), dto.UserToken);
        Assert.Equal(default(Guid), dto.GroupId);
    }
}

