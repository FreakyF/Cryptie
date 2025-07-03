using System;
using System.Collections.Generic;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetGroupMessagesSinceResponseDtoTests
{
    [Fact]
    public void MessageDto_Can_Set_And_Get_Properties()
    {
        // Arrange
        var messageId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var message = "Test message";
        var dateTime = DateTime.UtcNow;

        // Act
        var dto = new GetGroupMessagesSinceResponseDto.MessageDto
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
    public void GetGroupMessagesSinceResponseDto_Can_Set_And_Get_Messages()
    {
        // Arrange
        var messages = new List<GetGroupMessagesSinceResponseDto.MessageDto>
        {
            new GetGroupMessagesSinceResponseDto.MessageDto
            {
                MessageId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Message = "Test1",
                DateTime = DateTime.UtcNow
            },
            new GetGroupMessagesSinceResponseDto.MessageDto
            {
                MessageId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Message = "Test2",
                DateTime = DateTime.UtcNow
            }
        };

        // Act
        var response = new GetGroupMessagesSinceResponseDto { Messages = messages };

        // Assert
        Assert.Equal(messages, response.Messages);
    }

    [Fact]
    public void GetGroupMessagesSinceResponseDto_Messages_Can_Be_Null()
    {
        // Act
        var response = new GetGroupMessagesSinceResponseDto { Messages = null };
        // Assert
        Assert.Null(response.Messages);
    }

    [Fact]
    public void GetGroupMessagesSinceResponseDto_Default_Messages_Is_Null()
    {
        // Act
        var response = new GetGroupMessagesSinceResponseDto();
        // Assert
        Assert.NotNull(response.Messages);
    }
}

