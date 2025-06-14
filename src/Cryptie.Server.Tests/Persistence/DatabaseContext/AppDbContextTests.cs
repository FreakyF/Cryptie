using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.Honeypot;
using Cryptie.Common.Entities.LoginPolicy;
using Cryptie.Common.Entities.SessionTokens;
using Cryptie.Common.Entities.User;
using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Tests.Persistence.DatabaseContext;

public class AppDbContextTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void Can_Create_DbContext()
    {
        using var context = CreateContext();

        Assert.NotNull(context);
    }

    [Fact]
    public void Can_Access_DbSets()
    {
        using var context = CreateContext();

        Assert.NotNull(context.Users);
        Assert.NotNull(context.Groups);
        Assert.NotNull(context.GroupMessages);
        Assert.NotNull(context.UserGroupPublicKeys);
        Assert.NotNull(context.Passwords);
        Assert.NotNull(context.Totps);
        Assert.NotNull(context.PrivateKeys);
        Assert.NotNull(context.UserTokens);
        Assert.NotNull(context.TotpTokens);
        Assert.NotNull(context.UserLoginAttempts);
        Assert.NotNull(context.UserLoginHoneypotAttempts);
        Assert.NotNull(context.UserAccountLocks);
        Assert.NotNull(context.UserAccountHoneypotLocks);
    }

    [Fact]
    public void Can_Insert_And_Query_Group()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        const string name = "TestGroup";

        var entity = new Group { Id = id, Name = name };

        context.Groups.Add(entity);
        context.SaveChanges();

        var result = context.Groups.Find(id);

        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
    }

    [Fact]
    public void Can_Insert_And_Query_GroupMessage()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();

        var from = new User
        {
            Id = Guid.NewGuid(),
            Login = "f",
            DisplayName = "F",
            Email = "f@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        var to = new User
        {
            Id = Guid.NewGuid(),
            Login = "t",
            DisplayName = "T",
            Email = "t@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.AddRange(from, to);
        context.SaveChanges();

        var entity = new GroupMessage
        {
            Id = id,
            ReferenceId = Guid.NewGuid(),
            FromUser = from,
            ToUser = to,
            Message = "Hello",
            DateTime = DateTime.UtcNow
        };

        context.GroupMessages.Add(entity);
        context.SaveChanges();

        var result = context.GroupMessages.Find(id);

        Assert.NotNull(result);
        Assert.Equal(entity.Message, result.Message);
        Assert.Equal(from.Id, result.FromUser.Id);
        Assert.Equal(to.Id, result.ToUser.Id);
    }

    [Fact]
    public void Can_Insert_And_Query_UserGroupPublicKey()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(user);
        context.SaveChanges();

        const string key = "p";

        var entity = new UserGroupPublicKey { Id = id, User = user, Key = key };

        context.UserGroupPublicKeys.Add(entity);
        context.SaveChanges();

        var result = context.UserGroupPublicKeys.Find(id);
        Assert.NotNull(result);
        Assert.Equal(key, result.Key);
        Assert.Equal(user.Id, result.User.Id);
    }

    [Fact]
    public void Can_Insert_And_Query_User()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();

        var entity = new User
        {
            Id = id,
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(entity);
        context.SaveChanges();

        var result = context.Users.Find(id);
        Assert.NotNull(result);
    }

    [Fact]
    public void Can_Insert_And_Query_Password()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        const string secret = "p";

        var entity = new Password { Id = id, Secret = secret };

        context.Passwords.Add(entity);
        context.SaveChanges();

        var result = context.Passwords.Find(id);
        Assert.NotNull(result);
        Assert.Equal(secret, result.Secret);
    }

    [Fact]
    public void Can_Insert_And_Query_Totp()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        byte[] secret = [1];

        var entity = new Totp { Id = id, Secret = secret };

        context.Totps.Add(entity);
        context.SaveChanges();

        var result = context.Totps.Find(id);
        Assert.NotNull(result);
        Assert.Equal(secret, result.Secret);
    }

    [Fact]
    public void Can_Insert_And_Query_PrivateKey()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        var groupGuid = Guid.NewGuid();
        const string key = "p";

        var entity = new PrivateKey { Id = id, GroupGuid = groupGuid, Key = key };

        context.PrivateKeys.Add(entity);
        context.SaveChanges();

        var result = context.PrivateKeys.Find(id);
        Assert.NotNull(result);
        Assert.Equal(groupGuid, result.GroupGuid);
        Assert.Equal(key, result.Key);
    }

    [Fact]
    public void Can_Insert_And_Query_UserToken()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(user);
        context.SaveChanges();

        var entity = new UserToken { Id = id, User = user };
        context.UserTokens.Add(entity);
        context.SaveChanges();

        var result = context.UserTokens.Find(id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.User.Id);
    }

    [Fact]
    public void Can_Insert_And_Query_TotpToken()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        var time = DateTime.UtcNow.AddHours(1);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(user);
        context.SaveChanges();

        var entity = new TotpToken { Id = id, User = user, Until = time };
        context.TotpTokens.Add(entity);
        context.SaveChanges();

        var result = context.TotpTokens.Find(id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(time, result.Until);
    }

    [Fact]
    public void Can_Insert_And_Query_UserLoginAttempt()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(user);
        context.SaveChanges();

        var time = DateTime.UtcNow;
        var entity = new UserLoginAttempt { Id = id, User = user, TimeStamp = time };
        context.UserLoginAttempts.Add(entity);
        context.SaveChanges();

        var result = context.UserLoginAttempts.Find(id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(time, result.TimeStamp);
    }

    [Fact]
    public void Can_Insert_And_Query_UserLoginHoneypotAttempt()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        const string user = "u";
        var time = DateTime.UtcNow;

        var entity = new UserLoginHoneypotAttempt { Id = id, User = user, TimeStamp = time };

        context.UserLoginHoneypotAttempts.Add(entity);
        context.SaveChanges();

        var result = context.UserLoginHoneypotAttempts.Find(id);

        Assert.NotNull(result);
        Assert.Equal(user, result.User);
        Assert.Equal(time, result.TimeStamp);
    }

    [Fact]
    public void Can_Insert_And_Query_UserAccountLock()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = "u",
            DisplayName = "U",
            Email = "u@x.com",
            Password = new Password { Id = Guid.NewGuid(), Secret = "p" },
            Totp = new Totp { Id = Guid.NewGuid(), Secret = [1] }
        };

        context.Users.Add(user);
        context.SaveChanges();

        var until = DateTime.UtcNow.AddMinutes(5);
        var entity = new UserAccountLock { Id = id, User = user, Until = until };
        context.UserAccountLocks.Add(entity);
        context.SaveChanges();

        var result = context.UserAccountLocks.Find(id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(until, result.Until);
    }

    [Fact]
    public void Can_Insert_And_Query_UserAccountHoneypotLock()
    {
        using var context = CreateContext();
        var id = Guid.NewGuid();
        var time = DateTime.UtcNow.AddHours(1);
        const string user = "u";

        var entity = new UserAccountHoneypotLock { Id = id, User = user, Until = time };

        context.UserAccountHoneypotLocks.Add(entity);
        context.SaveChanges();

        var result = context.UserAccountHoneypotLocks.Find(id);
        Assert.NotNull(result);
        Assert.Equal(user, result.User);
        Assert.Equal(time, result.Until);
    }
}