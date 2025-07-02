using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("user_account_honeypot_locks")]
public class HoneypotAccountLock
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("username")] public string Username { get; set; } = string.Empty;

    [Required, Column("until")] public DateTime Until { get; set; }
}