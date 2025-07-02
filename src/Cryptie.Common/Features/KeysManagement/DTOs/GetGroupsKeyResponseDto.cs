namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class GetGroupsKeyResponseDto
{
    public Dictionary<Guid, string> Keys { get; set; }
}