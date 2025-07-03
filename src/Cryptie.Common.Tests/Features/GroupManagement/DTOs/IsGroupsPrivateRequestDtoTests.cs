using System;
using System.Collections.Generic;
using Cryptie.Common.Features.GroupManagement;
using Xunit;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class IsGroupsPrivateRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_GroupIds()
    {
        // Arrange
        var groupIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var dto = new IsGroupsPrivateRequestDto { GroupIds = groupIds };
        // Act & Assert
        Assert.Equal(groupIds, dto.GroupIds);
    }

    [Fact]
    public void GroupIds_Can_Be_Null()
    {
        // Arrange & Act
        var dto = new IsGroupsPrivateRequestDto { GroupIds = null };
        // Assert
        Assert.Null(dto.GroupIds);
    }

    [Fact]
    public void Default_GroupIds_Is_Null()
    {
        // Arrange & Act
        var dto = new IsGroupsPrivateRequestDto();
        // Assert
        Assert.NotNull(dto.GroupIds);
    }
}

