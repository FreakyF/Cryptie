using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("group_messages")]
public class GroupMessage
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("message")] public string Message { get; set; } = default!;

    [Required, Column("datetime")] public DateTime DateTime { get; set; }

    [Column("group_id")] public Guid? GroupId { get; set; }

    [ForeignKey(nameof(GroupId))] public Group? Group { get; set; }
}