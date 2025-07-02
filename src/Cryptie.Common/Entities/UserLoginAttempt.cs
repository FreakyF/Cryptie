using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("user_login_attempts")]
public class UserLoginAttempt
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("timestamp")] public DateTime Timestamp { get; set; }

    [Required, Column("user_id")] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; } = new();
}