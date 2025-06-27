using Cryptie.Common.Entities.Group;
using Cryptie.Common.Entities.Honeypot;
using Cryptie.Common.Entities.LoginPolicy;
using Cryptie.Common.Entities.SessionTokens;
using Cryptie.Common.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Cryptie.Server.Persistence.DatabaseContext;

public class AppDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions), IAppDbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMessage> GroupMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Password> Passwords { get; set; }
    public DbSet<Totp> Totps { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<TotpToken> TotpTokens { get; set; }
    public DbSet<UserLoginAttempt> UserLoginAttempts { get; set; }
    public DbSet<UserLoginHoneypotAttempt> UserLoginHoneypotAttempts { get; set; }
    public DbSet<UserAccountLock> UserAccountLocks { get; set; }
    public DbSet<UserAccountHoneypotLock> UserAccountHoneypotLocks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=55123;Database=cryptie;Username=postgres;Password=admin");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Groups)
            .WithMany(g => g.Users)
            .UsingEntity<Dictionary<string, object>>(
                // nazwa tabeli pośredniej
                "UserGroup",
                // ← relacja z kluczem obcym do Group
                join => join
                    .HasOne<Group>()
                    .WithMany()
                    .HasForeignKey("GroupId")
                    .OnDelete(DeleteBehavior.Cascade),
                // → relacja z kluczem obcym do User
                join => join
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                // konfiguracja samej tabeli łącznikowej
                join =>
                {
                    join.HasKey("UserId", "GroupId");
                    join.ToTable("UserGroups");
                });
        modelBuilder.Entity<User>()
            .HasMany(u => u.Friends)
            .WithMany() // brak nawigacji zwrotnej – przy self-relacji nie jest potrzebna
            .UsingEntity<Dictionary<string, object>>(
                "UserFriend",
                // FriendId – kasujemy wyłącznie sam rekord z tabeli łącznikowej
                join => join
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("FriendId")
                    .OnDelete(DeleteBehavior.Restrict),
                // UserId – normalnie kaskadujemy
                join => join
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("UserId", "FriendId");
                    join.ToTable("UserFriend");
                });
    }
}