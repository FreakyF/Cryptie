using Cryptie.Common.Entities;

namespace Cryptie.Common.Tests.Entities;

public class UserLoginAttemptTests
{
    [Fact]
    public void Can_Create_UserLoginAttempt_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        var attempt = new UserLoginAttempt
        {
            Id = id,
            Timestamp = timestamp,
            UserId = userId,
            User = user
        };

        // Assert
        Assert.Equal(id, attempt.Id);
        Assert.Equal(timestamp, attempt.Timestamp);
        Assert.Equal(userId, attempt.UserId);
        Assert.Equal(user, attempt.User);
    }

    [Fact]
    public void User_Property_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var attempt = new UserLoginAttempt();
        // Assert
        Assert.NotNull(attempt.User);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var attempt = new UserLoginAttempt();
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow.AddMinutes(5);
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };

        // Act
        attempt.Id = id;
        attempt.Timestamp = timestamp;
        attempt.UserId = userId;
        attempt.User = user;

        // Assert
        Assert.Equal(id, attempt.Id);
        Assert.Equal(timestamp, attempt.Timestamp);
        Assert.Equal(userId, attempt.UserId);
        Assert.Equal(user, attempt.User);
    }
}
