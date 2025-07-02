using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("user_login_honeypot_attempts")]
public class HoneypotLoginAttempt
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("username")] public string Username { get; set; } = string.Empty;

    [Required, Column("timestamp")] public DateTime Timestamp { get; set; }
}