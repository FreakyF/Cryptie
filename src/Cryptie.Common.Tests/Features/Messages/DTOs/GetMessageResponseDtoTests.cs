using System;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetMessageResponseDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var message = "Test message";
        var dateTime = DateTime.UtcNow;
        // Act
        var dto = new GetMessageResponseDto
        {
            MessageId = messageId,
            GroupId = groupId,
            SenderId = senderId,
            Message = message,
            DateTime = dateTime
        };
        // Assert
        Assert.Equal(messageId, dto.MessageId);
        Assert.Equal(groupId, dto.GroupId);
        Assert.Equal(senderId, dto.SenderId);
        Assert.Equal(message, dto.Message);
        Assert.Equal(dateTime, dto.DateTime);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new GetMessageResponseDto();
        // Assert
        Assert.Equal(Guid.Empty, dto.MessageId);
        Assert.Equal(Guid.Empty, dto.GroupId);
        Assert.Equal(Guid.Empty, dto.SenderId);
        Assert.NotNull(dto.Message);
        Assert.Equal(default(DateTime), dto.DateTime);
    }
}

