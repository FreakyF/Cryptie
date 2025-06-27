using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("users")]
public class User
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("login")] public string Login { get; set; } = default!;

    [Required, Column("display_name")] public string DisplayName { get; set; } = default!;

    [Required, Column("email")] public string Email { get; set; } = default!;

    [Required, Column("private_key")] public string PrivateKey { get; set; } = default!;


    [Required, Column("password_id")] public Guid PasswordId { get; set; }

    [Required, Column("totp_id")] public Guid TotpId { get; set; }

    [ForeignKey(nameof(PasswordId))] public Password Password { get; set; } = default!;

    [ForeignKey(nameof(TotpId))] public Totp Totp { get; set; } = default!;


    [InverseProperty(nameof(FriendsOf))] public ICollection<User> Friends { get; set; } = new HashSet<User>();

    [InverseProperty(nameof(Friends))] public ICollection<User> FriendsOf { get; set; } = new HashSet<User>();


    public ICollection<Group> Groups { get; set; } = new HashSet<Group>();


    public ICollection<UserToken> Tokens { get; set; } = new HashSet<UserToken>();
    public ICollection<TotpToken> TotpTokens { get; set; } = new HashSet<TotpToken>();
    public ICollection<UserAccountLock> AccountLocks { get; set; } = new HashSet<UserAccountLock>();
    public ICollection<UserLoginAttempt> LoginAttempts { get; set; } = new HashSet<UserLoginAttempt>();
    public ICollection<UserGroupKey> GroupKeys { get; set; } = new HashSet<UserGroupKey>();
}