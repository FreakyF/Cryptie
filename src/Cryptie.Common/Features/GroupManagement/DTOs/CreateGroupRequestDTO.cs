namespace Cryptie.Common.Features.GroupManagement;

public class CreateGroupRequestDto
{
    public Guid SessionToken { get; set; }
    public string Name { get; set; }
}