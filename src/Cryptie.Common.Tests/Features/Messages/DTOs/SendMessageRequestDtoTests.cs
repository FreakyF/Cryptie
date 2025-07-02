using System;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class SendMessageRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var message = "Test message";
        var groupId = Guid.NewGuid();
        var senderToken = Guid.NewGuid();
        // Act
        var dto = new SendMessageRequestDto
        {
            Message = message,
            GroupId = groupId,
            SenderToken = senderToken
        };
        // Assert
        Assert.Equal(message, dto.Message);
        Assert.Equal(groupId, dto.GroupId);
        Assert.Equal(senderToken, dto.SenderToken);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new SendMessageRequestDto();
        // Assert
        Assert.Null(dto.Message);
        Assert.Equal(default(Guid), dto.GroupId);
        Assert.Equal(default(Guid), dto.SenderToken);
    }
}

