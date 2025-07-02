using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class UserTokenTests
{
    [Fact]
    public void Can_Create_UserToken_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        var token = new UserToken
        {
            Id = id,
            UserId = userId,
            User = user
        };

        // Assert
        Assert.Equal(id, token.Id);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(user, token.User);
    }

    [Fact]
    public void User_Property_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var token = new UserToken();
        // Assert
        Assert.NotNull(token.User);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var token = new UserToken();
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        token.Id = id;
        token.UserId = userId;
        token.User = user;

        // Assert
        Assert.Equal(id, token.Id);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(user, token.User);
    }
}
