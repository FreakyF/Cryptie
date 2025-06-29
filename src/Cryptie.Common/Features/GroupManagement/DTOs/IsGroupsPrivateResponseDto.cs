namespace Cryptie.Common.Features.GroupManagement;

public class IsGroupsPrivateResponseDto
{
    public Dictionary<Guid, bool> GroupStatuses { get; set; }
}