using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.Honeypot;
using Cryptie.Common.Entities.LoginPolicy;
using Cryptie.Common.Entities.SessionTokens;
using Cryptie.Common.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Persistence.DatabaseContext;

public interface IAppDbContext : IDisposable
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMessage> GroupMessages { get; set; }
    public DbSet<UserGroupPublicKey> UserGroupPublicKeys { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Password> Passwords { get; set; }
    public DbSet<Totp> Totps { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<TotpToken> TotpTokens { get; set; }
    public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
    public DbSet<UserLoginHoneypotAttempt> UserLoginHoneypotAttempts { get; set; }
    public DbSet<UserAccountLock> UserAccountLocks { get; set; }
    public DbSet<UserAccountHoneypotLock> UserAccountHoneypotLocks { get; set; }
    
    public int SaveChanges();
    public int SaveChanges(bool acceptAllChangesOnSuccess);
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
}