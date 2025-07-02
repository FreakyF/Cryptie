using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class UserAccountLockTests
{
    [Fact]
    public void Can_Create_UserAccountLock_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var until = DateTime.UtcNow.AddMinutes(30);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        var lockObj = new UserAccountLock
        {
            Id = id,
            Until = until,
            UserId = userId,
            User = user
        };

        // Assert
        Assert.Equal(id, lockObj.Id);
        Assert.Equal(until, lockObj.Until);
        Assert.Equal(userId, lockObj.UserId);
        Assert.Equal(user, lockObj.User);
    }

    [Fact]
    public void User_Property_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var lockObj = new UserAccountLock();
        // Assert
        Assert.NotNull(lockObj.User);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var lockObj = new UserAccountLock();
        var id = Guid.NewGuid();
        var until = DateTime.UtcNow.AddHours(1);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        lockObj.Id = id;
        lockObj.Until = until;
        lockObj.UserId = userId;
        lockObj.User = user;

        // Assert
        Assert.Equal(id, lockObj.Id);
        Assert.Equal(until, lockObj.Until);
        Assert.Equal(userId, lockObj.UserId);
        Assert.Equal(user, lockObj.User);
    }
}
