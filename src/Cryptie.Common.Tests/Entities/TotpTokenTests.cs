using System;
using Cryptie.Common.Entities;

namespace Cryptie.Common.Tests.Entities;

public class TotpTokenTests
{
    [Fact]
    public void Can_Create_TotpToken_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var until = DateTime.UtcNow.AddMinutes(5);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        var token = new TotpToken
        {
            Id = id,
            Until = until,
            UserId = userId,
            User = user
        };

        // Assert
        Assert.Equal(id, token.Id);
        Assert.Equal(until, token.Until);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(user, token.User);
    }

    [Fact]
    public void User_Property_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var token = new TotpToken();

        // Assert
        Assert.NotNull(token.User);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var token = new TotpToken();
        var id = Guid.NewGuid();
        var until = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        token.Id = id;
        token.Until = until;
        token.UserId = userId;
        token.User = user;

        // Assert
        Assert.Equal(id, token.Id);
        Assert.Equal(until, token.Until);
        Assert.Equal(userId, token.UserId);
        Assert.Equal(user, token.User);
    }
}
