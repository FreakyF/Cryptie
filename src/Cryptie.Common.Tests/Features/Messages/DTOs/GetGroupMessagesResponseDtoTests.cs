using System;
using System.Collections.Generic;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetGroupMessagesResponseDtoTests
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
        var dto = new GetGroupMessagesResponseDto.MessageDto
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
    public void GetGroupMessagesResponseDto_Can_Set_And_Get_Messages()
    {
        // Arrange
        var messages = new List<GetGroupMessagesResponseDto.MessageDto>
        {
            new GetGroupMessagesResponseDto.MessageDto
            {
                MessageId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Message = "Test1",
                DateTime = DateTime.UtcNow
            },
            new GetGroupMessagesResponseDto.MessageDto
            {
                MessageId = Guid.NewGuid(),
                GroupId = Guid.NewGuid(),
                SenderId = Guid.NewGuid(),
                Message = "Test2",
                DateTime = DateTime.UtcNow
            }
        };

        // Act
        var response = new GetGroupMessagesResponseDto { Messages = messages };

        // Assert
        Assert.Equal(messages, response.Messages);
    }

    [Fact]
    public void GetGroupMessagesResponseDto_Messages_Can_Be_Null()
    {
        // Act
        var response = new GetGroupMessagesResponseDto { Messages = null };
        // Assert
        Assert.Null(response.Messages);
    }

    [Fact]
    public void GetGroupMessagesResponseDto_Default_Messages_Is_Null()
    {
        // Act
        var response = new GetGroupMessagesResponseDto();
        // Assert
        Assert.NotNull(response.Messages);
    }
}

