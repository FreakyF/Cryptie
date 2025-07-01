using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("group_encryption_keys")]
public class GroupEncryptionKey
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [Required] [Column("group_id")] public Guid GroupId { get; set; }

    [Required] [Column("user_id")] public Guid UserId { get; set; }

    [Required] [Column("key")] public string AesKey { get; set; } = null!;

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; } = null!;

    [ForeignKey(nameof(UserId))] public User User { get; set; } = null!;
}