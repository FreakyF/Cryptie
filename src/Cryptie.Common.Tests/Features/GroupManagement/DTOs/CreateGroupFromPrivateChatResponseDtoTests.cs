using System;
using Cryptie.Common.Features.GroupManagement;
using Xunit;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class CreateGroupFromPrivateChatResponseDtoTests
{
    [Fact]
    public void Can_Set_And_Get_GroupId()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var dto = new CreateGroupFromPrivateChatResponseDto();
        // Act
        dto.GroupId = groupId;
        // Assert
        Assert.Equal(groupId, dto.GroupId);
    }

    [Fact]
    public void Default_GroupId_Is_Empty()
    {
        // Act
        var dto = new CreateGroupFromPrivateChatResponseDto();
        // Assert
        Assert.Equal(Guid.Empty, dto.GroupId);
    }
}

