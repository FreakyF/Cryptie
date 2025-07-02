using System;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class HoneypotLoginAttemptTests
{
    [Fact]
    public void Can_Create_HoneypotLoginAttempt_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "testuser";
        var timestamp = DateTime.UtcNow;

        // Act
        var attempt = new HoneypotLoginAttempt
        {
            Id = id,
            Username = username,
            Timestamp = timestamp
        };

        // Assert
        Assert.Equal(id, attempt.Id);
        Assert.Equal(username, attempt.Username);
        Assert.Equal(timestamp, attempt.Timestamp);
    }

    [Fact]
    public void Username_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var attempt = new HoneypotLoginAttempt();
        // Assert
        Assert.NotNull(attempt.Username);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var attempt = new HoneypotLoginAttempt();
        var id = Guid.NewGuid();
        var username = "anotheruser";
        var timestamp = DateTime.UtcNow.AddMinutes(5);

        // Act
        attempt.Id = id;
        attempt.Username = username;
        attempt.Timestamp = timestamp;

        // Assert
        Assert.Equal(id, attempt.Id);
        Assert.Equal(username, attempt.Username);
        Assert.Equal(timestamp, attempt.Timestamp);
    }
}
