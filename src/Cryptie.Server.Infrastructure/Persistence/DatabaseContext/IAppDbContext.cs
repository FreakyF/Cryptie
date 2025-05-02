using Cryptie.Server.Domain.Features.Authentication.Entities.Chat;
using Cryptie.Server.Domain.Features.Authentication.Entities.Group;
using Cryptie.Server.Domain.Features.Authentication.Entities.Honeypot;
using Cryptie.Server.Domain.Features.Authentication.Entities.LoginPolicy;
using Cryptie.Server.Domain.Features.Authentication.Entities.SessionTokens;
using Cryptie.Server.Domain.Features.Authentication.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Infrastructure.Persistence.DatabaseContext;

public interface IAppDbContext : IDisposable
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMessages> ChatMessages { get; set; }
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
}