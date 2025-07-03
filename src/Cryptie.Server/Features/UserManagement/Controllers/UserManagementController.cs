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
/// <summary>
/// Resolves a user's GUID from a session token.
/// </summary>
/// <param name="userGuidFromTokenRequest">Request containing the token.</param>
/// <returns>User identifier if the token is valid.</returns>
public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest)
{
    return userManagementService.UserGuidFromToken(userGuidFromTokenRequest);
}

[HttpPost("addfriend", Name = "PostAddFriend")]
/// <summary>
/// Adds a new friend and establishes a private chat.
/// </summary>
/// <param name="addFriendRequest">Friend identifier and encryption keys.</param>
/// <returns>HTTP 200 on success.</returns>
public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest)
{
    return userManagementService.AddFriend(addFriendRequest);
}

[HttpGet("namefromguid", Name = "GetNameFromGuid")]
/// <summary>
/// Retrieves the display name of a user from their GUID.
/// </summary>
/// <param name="nameFromGuidRequest">The request containing the GUID.</param>
/// <returns>User display name.</returns>
public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest)
{
    return userManagementService.NameFromGuid(nameFromGuidRequest);
}
    
[HttpGet("guidfromlogin", Name = "GetGuidFromLogin")]
/// <summary>
/// Looks up a user's GUID by their login name.
/// </summary>
/// <param name="guidFromLoginRequest">Request containing the login.</param>
/// <returns>User identifier or 404.</returns>
public IActionResult GuidFromLogin([FromBody] GuidFromLoginRequestDto guidFromLoginRequest)
{
    return userManagementService.GuidFromLogin(guidFromLoginRequest);
}

[HttpGet("usergroups", Name = "GetUserGroups")]
/// <summary>
/// Lists all groups that the authenticated user belongs to.
/// </summary>
/// <param name="userGroupsRequest">Request containing the session token.</param>
/// <returns>Collection of group ids.</returns>
public IActionResult UserGroups([FromBody] UserGroupsRequestDto userGroupsRequest)
{
    return userManagementService.UserGroups(userGroupsRequest);
}

[HttpPost("userdisplayname", Name = "ChangeUserDisplayName")]
/// <summary>
/// Changes the display name of the authenticated user.
/// </summary>
/// <param name="userDisplayNameRequest">Token and new display name.</param>
/// <returns>HTTP 200 on success.</returns>
public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
{
    return userManagementService.UserDisplayName(userDisplayNameRequest);
}
    
[HttpGet("privateKey", Name = "UserPrivateKey")]
/// <summary>
/// Retrieves the private key of the authenticated user.
/// </summary>
/// <param name="userPrivateKeyRequest">Request containing the token.</param>
/// <returns>Private key pair.</returns>
public IActionResult UserPrivateKey([FromBody] UserPrivateKeyRequestDto userPrivateKeyRequest)
{
    return userManagementService.UserPrivateKey(userPrivateKeyRequest);
}
}