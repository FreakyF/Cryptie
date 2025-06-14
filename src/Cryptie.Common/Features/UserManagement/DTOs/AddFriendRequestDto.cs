namespace Cryptie.Common.Features.UserManagement.DTOs;

public class AddFriendRequestDto
{
    public Guid SessionToken { get; set; }
    public Guid Friend { get; set; }
}