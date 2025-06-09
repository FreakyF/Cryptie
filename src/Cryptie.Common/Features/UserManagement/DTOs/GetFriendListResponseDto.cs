using Cryptie.Common.Entities.User;

namespace Cryptie.Common.Features.UserManagement.DTOs;

public class GetFriendListResponseDto
{
    public List<Guid> Friends { get; set; }
}