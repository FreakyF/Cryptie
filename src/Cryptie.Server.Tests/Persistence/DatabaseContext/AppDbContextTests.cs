using Cryptie.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Cryptie.Common.Entities;

namespace Cryptie.Server.Tests.Persistence.DatabaseContext
{
    public class AppDbContextTests
    {
        private static DbContextOptions<AppDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb" + System.Guid.NewGuid())
                .Options;
        }

        [Fact]
        public void CanInstantiateDbContext()
        {
            var options = CreateInMemoryOptions();
            using var context = new AppDbContext(options);
            Assert.NotNull(context);
        }

        [Fact]
        public void DbSets_AreAccessible()
        {
            var options = CreateInMemoryOptions();
            using var context = new AppDbContext(options);
            Assert.NotNull(context.Groups);
            Assert.NotNull(context.GroupMessages);
            Assert.NotNull(context.Users);
            Assert.NotNull(context.Passwords);
            Assert.NotNull(context.Totps);
            Assert.NotNull(context.UserTokens);
            Assert.NotNull(context.TotpTokens);
            Assert.NotNull(context.UserLoginAttempts);
            Assert.NotNull(context.HoneypotLoginAttempts);
            Assert.NotNull(context.UserAccountLocks);
            Assert.NotNull(context.HoneypotAccountLocks);
        }

        [Fact]
        public void CanAddAndRetrieveEntities()
        {
            var options = CreateInMemoryOptions();
            var userId = Guid.NewGuid();
            using (var context = new AppDbContext(options))
            {
                var user = new User {
                    Id = userId,
                    Login = "testuser",
                    DisplayName = "Test User",
                    Email = "test@example.com",
                    PrivateKey = "privkey",
                    PublicKey = "pubkey",
                    PasswordId = Guid.NewGuid(),
                    TotpId = Guid.NewGuid(),
                    Password = new Password { Id = Guid.NewGuid(), Secret = "hash" },
                    Totp = new Totp { Id = Guid.NewGuid(), Secret = "secret"u8.ToArray() },
                    ControlValue = "test"
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
            using (var context = new AppDbContext(options))
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                Assert.NotNull(user);
                Assert.Equal("testuser", user.Login);
            }
        }
    }
}
