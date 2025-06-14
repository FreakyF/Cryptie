namespace Cryptie.Common.Features.GroupManagement;

public class AddUserToGroupRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid GroupGuid { get; set; }
    public Guid UserToAdd { get; set; }
}