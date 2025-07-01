namespace Cryptie.Common.Features.UserManagement.DTOs;

public class AddFriendRequestDto
{
    public Guid SessionToken { get; set; }
    public string Friend { get; set; }
    public Dictionary<Guid, string> EncryptionKeys { get; set; } = new();
}