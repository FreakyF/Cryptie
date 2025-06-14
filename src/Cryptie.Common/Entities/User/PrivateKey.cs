namespace Cryptie.Common.Entities.User;

public class PrivateKey
{
    public Guid Id { get; init; }
    public Guid GroupGuid { get; set; }
    public string Key { get; set; }
}