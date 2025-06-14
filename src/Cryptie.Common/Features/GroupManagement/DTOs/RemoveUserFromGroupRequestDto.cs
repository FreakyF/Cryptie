namespace Cryptie.Common.Features.GroupManagement;

public class RemoveUserFromGroupRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid GroupGuid { get; set; }
    public Guid UserToRemove { get; set; }
}