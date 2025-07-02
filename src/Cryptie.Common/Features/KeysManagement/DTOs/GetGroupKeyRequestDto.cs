namespace Cryptie.Common.Features.KeysManagement.DTOs;

public class GetGroupKeyRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid GroupId { get; set; }
}