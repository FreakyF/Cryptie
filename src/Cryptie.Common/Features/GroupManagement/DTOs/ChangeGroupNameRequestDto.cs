namespace Cryptie.Common.Features.GroupManagement;

public class ChangeGroupNameRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid GroupGuid { get; set; }
    public string NewName { get; set; } = string.Empty;
}