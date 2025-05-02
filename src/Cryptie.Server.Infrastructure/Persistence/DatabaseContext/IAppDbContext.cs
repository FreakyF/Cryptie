using Cryptie.Server.Domain.Features.Authentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Infrastructure.Persistence.DatabaseContext;

public interface IAppDbContext : IDisposable
{
    public DbSet<User> Users { get; set; }
    public DbSet<Password> Passwords { get; set; }
    public DbSet<Totp> Totps { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<TotpToken> TotpTokens { get; set; }
    public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
    public DbSet<UserLoginHoneypotAttempt> UserLoginHoneypotAttempts { get; set; }
    public DbSet<UserAccountLock> UserAccountLocks { get; set; }
    public DbSet<UserAccountHoneypotLock> UserAccountHoneypotLocks { get; set; }
}