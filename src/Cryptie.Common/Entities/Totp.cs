using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptie.Common.Entities;

[Table("totps")]
public class Totp
{
    [Key, Column("id")] public Guid Id { get; set; }

    [Required, Column("secret")] public byte[] Secret { get; set; } = Array.Empty<byte>();


    public ICollection<User> Users { get; set; } = new HashSet<User>();
}