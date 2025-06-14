namespace Cryptie.Common.Features.GroupManagement;

public class DeleteGroupRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid GroupGuid { get; set; }
}