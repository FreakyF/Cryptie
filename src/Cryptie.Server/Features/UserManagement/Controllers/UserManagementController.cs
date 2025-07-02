using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Features.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement.Controllers;

[ApiController]
[Route("user")]
public class UserManagementController(IUserManagementService userManagementService)
    : ControllerBase
{
    [HttpGet("guid", Name = "GetUserGuidFromToken")]
    public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest)
    {
        return userManagementService.UserGuidFromToken(userGuidFromTokenRequest);
    }

    [HttpGet("login", Name = "GetUserLoginFromToken")]
    public IActionResult UserLoginFromToken([FromBody] UserLoginFromTokenRequestDto userLoginFromTokenRequest)
    {
        return userManagementService.UserLoginFromToken(userLoginFromTokenRequest);
    }

    [HttpPost("addfriend", Name = "PostAddFriend")]
    public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest)
    {
        return userManagementService.AddFriend(addFriendRequest);
    }

    [HttpGet("friendlist", Name = "GetFriendList")]
    public IActionResult FriendList([FromBody] FriendListRequestDto friendListRequest)
    {
        return userManagementService.FriendList(friendListRequest);
    }

    [HttpGet("namefromguid", Name = "GetNameFromGuid")]
    public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest)
    {
        return userManagementService.NameFromGuid(nameFromGuidRequest);
    }

    [HttpGet("usergroups", Name = "GetUserGroups")]
    public IActionResult UserGroups([FromBody] UserGroupsRequestDto userGroupsRequest)
    {
        return userManagementService.UserGroups(userGroupsRequest);
    }

    [HttpPost("userdisplayname", Name = "ChangeUserDisplayName")]
    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
    {
        return userManagementService.UserDisplayName(userDisplayNameRequest);
    }
}