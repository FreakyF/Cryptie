namespace Cryptie.Common.Features.GroupManagement;

public class CreateGroupRequestDTO
{
    public Guid SessionToken { get; set; }
    public string Name { get; set; }
}