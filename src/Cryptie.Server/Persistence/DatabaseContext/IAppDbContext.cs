using Cryptie.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Persistence.DatabaseContext;

public interface IAppDbContext : IDisposable
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMessage> GroupMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Password> Passwords { get; set; }
    public DbSet<Totp> Totps { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<TotpToken> TotpTokens { get; set; }
    public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
    public DbSet<HoneypotLoginAttempt> HoneypotLoginAttempts { get; set; }
    public DbSet<UserAccountLock> UserAccountLocks { get; set; }
    public DbSet<HoneypotAccountLock> HoneypotAccountLocks { get; set; }

    public int SaveChanges();
    public int SaveChanges(bool acceptAllChangesOnSuccess);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
}