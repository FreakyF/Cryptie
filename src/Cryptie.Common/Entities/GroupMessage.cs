using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("group_messages")]
public class GroupMessage
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("message")] public string Message { get; set; } = string.Empty;

    [Required, Column("datetime")] public DateTime DateTime { get; set; }

    [Required, Column("group_id")] public Guid GroupId { get; set; }

    [ForeignKey(nameof(GroupId))] public Group Group { get; set; } = new Group();

    [Required, Column("sender_id")] public Guid SenderId { get; set; }

    [ForeignKey(nameof(SenderId))] public User Sender { get; set; } = new User();
}