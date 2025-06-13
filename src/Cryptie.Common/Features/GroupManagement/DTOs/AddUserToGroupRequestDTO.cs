namespace Cryptie.Common.Features.GroupManagement;

public class AddUserToGroupRequestDTO
{
    public Guid Token { get; set; }
    public Guid GroupGuid { get; set; }
    public Guid UserToAdd { get; set; }
}