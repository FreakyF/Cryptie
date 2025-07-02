namespace Cryptie.Common.Features.GroupManagement.DTOs;

public class GetGroupsNamesResponseDto
{
    public Dictionary<Guid, string> GroupsNames { get; set; }
}