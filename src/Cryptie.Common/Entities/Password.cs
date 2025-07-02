using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("passwords")]
public class Password
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("secret")] public string Secret { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new HashSet<User>();
}