using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("totp_tokens")]
public class TotpToken
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("until")] public DateTime Until { get; set; }

    [Required, Column("user_id")] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; } = default!;

    public TotpToken()
    {
        User = new User();
    }
}