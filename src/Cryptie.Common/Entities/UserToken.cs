using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("user_tokens")]
public class UserToken
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("user_id")] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public User User { get; set; } = default!;
}