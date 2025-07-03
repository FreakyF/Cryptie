namespace Cryptie.Common.Features.GroupManagement;

public class CreateGroupFromPrivateChatRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid PrivateChatId { get; set; }
    public string NewMember { get; set; } = string.Empty;
    public Dictionary<Guid, string> EncryptionKeys { get; set; } = new();
}