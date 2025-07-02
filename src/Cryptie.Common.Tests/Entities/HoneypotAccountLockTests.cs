using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class HoneypotAccountLockTests
{
    [Fact]
    public void Can_Create_HoneypotAccountLock_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "testuser";
        var until = DateTime.UtcNow.AddMinutes(10);

        // Act
        var lockObj = new HoneypotAccountLock
        {
            Id = id,
            Username = username,
            Until = until
        };

        // Assert
        Assert.Equal(id, lockObj.Id);
        Assert.Equal(username, lockObj.Username);
        Assert.Equal(until, lockObj.Until);
    }

    [Fact]
    public void Username_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var lockObj = new HoneypotAccountLock();
        // Assert
        Assert.NotNull(lockObj.Username);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var lockObj = new HoneypotAccountLock();
        var id = Guid.NewGuid();
        var username = "anotheruser";
        var until = DateTime.UtcNow.AddHours(1);

        // Act
        lockObj.Id = id;
        lockObj.Username = username;
        lockObj.Until = until;

        // Assert
        Assert.Equal(id, lockObj.Id);
        Assert.Equal(username, lockObj.Username);
        Assert.Equal(until, lockObj.Until);
    }
}
