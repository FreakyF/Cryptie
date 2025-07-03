using System;
using System.Collections.Generic;
using Cryptie.Common.Features.GroupManagement;
using Xunit;

namespace Cryptie.Common.Tests.Features.GroupManagement.DTOs;

public class IsGroupsPrivateResponseDtoTests
{
    [Fact]
    public void Can_Set_And_Get_GroupStatuses()
    {
        // Arrange
        var statuses = new Dictionary<Guid, bool>
        {
            { Guid.NewGuid(), true },
            { Guid.NewGuid(), false }
        };
        var dto = new IsGroupsPrivateResponseDto { GroupStatuses = statuses };
        // Act & Assert
        Assert.Equal(statuses, dto.GroupStatuses);
    }

    [Fact]
    public void GroupStatuses_Can_Be_Null()
    {
        // Arrange & Act
        var dto = new IsGroupsPrivateResponseDto { GroupStatuses = null };
        // Assert
        Assert.Null(dto.GroupStatuses);
    }

    [Fact]
    public void Default_GroupStatuses_Is_Null()
    {
        // Arrange & Act
        var dto = new IsGroupsPrivateResponseDto();
        // Assert
        Assert.NotNull(dto.GroupStatuses);
    }
}

