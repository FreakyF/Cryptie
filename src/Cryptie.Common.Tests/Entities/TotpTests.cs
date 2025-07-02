using System;
using System.Collections.Generic;
using Cryptie.Common.Entities;
using Xunit;

namespace Cryptie.Common.Tests.Entities;

public class TotpTests
{
    [Fact]
    public void Can_Create_Totp_With_Valid_Properties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var secret = new byte[] { 1, 2, 3, 4 };
        var users = new HashSet<User> { new User { Id = Guid.NewGuid() } };

        // Act
        var totp = new Totp
        {
            Id = id,
            Secret = secret,
            Users = users
        };

        // Assert
        Assert.Equal(id, totp.Id);
        Assert.Equal(secret, totp.Secret);
        Assert.Equal(users, totp.Users);
    }

    [Fact]
    public void Secret_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var totp = new Totp();
        // Assert
        Assert.NotNull(totp.Secret);
    }

    [Fact]
    public void Users_Should_Not_Be_Null_By_Default()
    {
        // Arrange
        var totp = new Totp();
        // Assert
        Assert.NotNull(totp.Users);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        // Arrange
        var totp = new Totp();
        var id = Guid.NewGuid();
        var secret = new byte[] { 5, 6, 7 };
        var users = new List<User> { new User { Id = Guid.NewGuid() } };

        // Act
        totp.Id = id;
        totp.Secret = secret;
        totp.Users = users;

        // Assert
        Assert.Equal(id, totp.Id);
        Assert.Equal(secret, totp.Secret);
        Assert.Equal(users, totp.Users);
    }
}
