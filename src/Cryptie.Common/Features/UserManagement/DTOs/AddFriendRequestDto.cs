namespace Cryptie.Common.Features.UserManagement.DTOs;

public class AddFriendRequestDto
{
    public Guid Token { get; set; }
    public Guid Friend { get; set; }
}