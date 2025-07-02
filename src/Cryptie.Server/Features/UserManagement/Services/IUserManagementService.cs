using Cryptie.Common.Features.UserManagement.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement.Services;

public interface IUserManagementService
{
    public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest);

    public IActionResult UserLoginFromToken([FromBody] UserLoginFromTokenRequestDto userLoginFromTokenRequest);

    public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest);

    public IActionResult FriendList([FromBody] FriendListRequestDto friendListRequest);

    public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest);

    public IActionResult UserGroups([FromBody] UserGroupsRequestDto userGroupsRequest);

    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest);
    public IActionResult UserPrivateKey([FromBody] UserPrivateKeyRequestDto userPrivateKeyRequest);
    public IActionResult GuidFromLogin([FromBody] GuidFromLoginRequestDto guidFromLoginRequest);
}