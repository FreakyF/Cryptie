using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("groups")]
public class Group
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("name")] public string Name { get; set; } = string.Empty;

    [Required, Column("is_private")] public bool IsPrivate { get; set; }

    public ICollection<User> Members { get; set; } = new HashSet<User>();

    public ICollection<GroupMessage> Messages { get; set; } = new HashSet<GroupMessage>();
    public ICollection<GroupEncryptionKey> GroupEncryptionKeys { get; set; } = new HashSet<GroupEncryptionKey>();
}