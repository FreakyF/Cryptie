using System;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetMessageRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var userToken = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        // Act
        var dto = new GetMessageRequestDto
        {
            UserToken = userToken,
            GroupId = groupId,
            MessageId = messageId
        };
        // Assert
        Assert.Equal(userToken, dto.UserToken);
        Assert.Equal(groupId, dto.GroupId);
        Assert.Equal(messageId, dto.MessageId);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new GetMessageRequestDto();
        // Assert
        Assert.Equal(default(Guid), dto.UserToken);
        Assert.Equal(default(Guid), dto.GroupId);
        Assert.Equal(default(Guid), dto.MessageId);
    }
}

