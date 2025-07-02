using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class GroupMessageTests
{
    [Fact]
    public void Can_Create_GroupMessage_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var message = "Test message";
        var dateTime = DateTime.UtcNow;
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId };
        var senderId = Guid.NewGuid();
        var sender = new User { Id = senderId };

        // Act
        var groupMessage = new GroupMessage
        {
            Id = id,
            Message = message,
            DateTime = dateTime,
            GroupId = groupId,
            Group = group,
            SenderId = senderId,
            Sender = sender
        };

        // Assert
        Assert.Equal(id, groupMessage.Id);
        Assert.Equal(message, groupMessage.Message);
        Assert.Equal(dateTime, groupMessage.DateTime);
        Assert.Equal(groupId, groupMessage.GroupId);
        Assert.Equal(group, groupMessage.Group);
        Assert.Equal(senderId, groupMessage.SenderId);
        Assert.Equal(sender, groupMessage.Sender);
    }

    [Fact]
    public void Message_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var groupMessage = new GroupMessage();
        // Assert
        Assert.NotNull(groupMessage.Message);
    }

    [Fact]
    public void Sender_Property_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var groupMessage = new GroupMessage();
        // Assert
        Assert.NotNull(groupMessage.Sender);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var groupMessage = new GroupMessage();
        var id = Guid.NewGuid();
        var message = "Another message";
        var dateTime = DateTime.UtcNow;
        var groupId = Guid.NewGuid();
        var group = new Group { Id = groupId };
        var senderId = Guid.NewGuid();
        var sender = new User { Id = senderId };

        // Act
        groupMessage.Id = id;
        groupMessage.Message = message;
        groupMessage.DateTime = dateTime;
        groupMessage.GroupId = groupId;
        groupMessage.Group = group;
        groupMessage.SenderId = senderId;
        groupMessage.Sender = sender;

        // Assert
        Assert.Equal(id, groupMessage.Id);
        Assert.Equal(message, groupMessage.Message);
        Assert.Equal(dateTime, groupMessage.DateTime);
        Assert.Equal(groupId, groupMessage.GroupId);
        Assert.Equal(group, groupMessage.Group);
        Assert.Equal(senderId, groupMessage.SenderId);
        Assert.Equal(sender, groupMessage.Sender);
    }
}
