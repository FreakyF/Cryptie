using Cryptie.Common.Entities;


namespace Cryptie.Common.Tests.Entities;

public class PasswordTests
{
    [Fact]
    public void Can_Create_Password_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var secret = "mySecret";
        var users = new HashSet<User> { new User { Id = Guid.NewGuid() } };

        // Act
        var password = new Password
        {
            Id = id,
            Secret = secret,
            Users = users
        };

        // Assert
        Assert.Equal(id, password.Id);
        Assert.Equal(secret, password.Secret);
        Assert.Equal(users, password.Users);
    }

    [Fact]
    public void Secret_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var password = new Password();

        // Assert
        Assert.NotNull(password.Secret);
    }

    [Fact]
    public void Users_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var password = new Password();

        // Assert
        Assert.NotNull(password.Users);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var password = new Password();
        var id = Guid.NewGuid();
        var secret = "anotherSecret";
        var users = new List<User> { new User { Id = Guid.NewGuid() } };

        // Act
        password.Id = id;
        password.Secret = secret;
        password.Users = users;

        // Assert
        Assert.Equal(id, password.Id);
        Assert.Equal(secret, password.Secret);
        Assert.Equal(users, password.Users);
    }
}
