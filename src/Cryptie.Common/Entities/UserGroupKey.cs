using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("user_group_keys")]
public class UserGroupKey
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("user_id")] public Guid UserId { get; set; }

    [Required, Column("group_id")] public Guid GroupId { get; set; }

    [Required, Column("key")] public string Key { get; set; } = default!;

    [ForeignKey(nameof(UserId))] public User User { get; set; } = default!;

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; } = default!;
}