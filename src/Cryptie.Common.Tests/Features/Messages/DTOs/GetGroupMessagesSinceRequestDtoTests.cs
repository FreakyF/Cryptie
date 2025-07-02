using System;
using Cryptie.Common.Features.Messages.DTOs;
using Xunit;

namespace Cryptie.Common.Tests.Features.Messages.DTOs;

public class GetGroupMessagesSinceRequestDtoTests
{
    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var userToken = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var since = DateTime.UtcNow;
        // Act
        var dto = new GetGroupMessagesSinceRequestDto
        {
            UserToken = userToken,
            GroupId = groupId,
            Since = since
        };
        // Assert
        Assert.Equal(userToken, dto.UserToken);
        Assert.Equal(groupId, dto.GroupId);
        Assert.Equal(since, dto.Since);
    }

    [Fact]
    public void Default_Values_Are_Default()
    {
        // Act
        var dto = new GetGroupMessagesSinceRequestDto();
        // Assert
        Assert.Equal(Guid.Empty, dto.UserToken);
        Assert.Equal(Guid.Empty, dto.GroupId);
        Assert.Equal(default(DateTime), dto.Since);
    }
}

