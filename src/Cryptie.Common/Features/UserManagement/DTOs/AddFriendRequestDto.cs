namespace Cryptie.Common.Features.UserManagement.DTOs;

public class AddFriendRequestDto
{
    public Guid SessionToken { get; set; }
    public string Friend { get; set; } = string.Empty;
    public Dictionary<Guid, string> EncryptionKeys { get; set; } = new();
}