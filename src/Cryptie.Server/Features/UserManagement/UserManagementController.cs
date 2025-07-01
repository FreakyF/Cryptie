using Cryptie.Common.Entities;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement;

[ApiController]
[Route("user")]
public class UserManagementController(IDatabaseService databaseService) : ControllerBase
{
    [HttpGet("guid", Name = "GetUserGuidFromToken")]
    public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest)
    {
        var user = databaseService.GetUserFromToken(userGuidFromTokenRequest.SessionToken);
        if (user == null) return BadRequest();

        return Ok(new UserGuidFromTokenResponseDto
        {
            Guid = user.Id
        });
    }

    [HttpGet("login", Name = "GetUserLoginFromToken")]
    public IActionResult UserLoginFromToken([FromBody] UserLoginFromTokenRequestDto userLoginFromTokenRequest)
    {
        var user = databaseService.GetUserFromToken(userLoginFromTokenRequest.SessionToken);
        if (user == null) return BadRequest();

        return Ok(new UserLoginFromTokenResponseDto
        {
            Login = user.Login
        });
    }

    [HttpPost("addfriend", Name = "PostAddFriend")]
    public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest)
    {
        var friend = databaseService.FindUserByLogin(addFriendRequest.Friend);
        if (friend == null) return NotFound();
        var user = databaseService.GetUserFromToken(addFriendRequest.SessionToken);
        if (user == null) return BadRequest();

        var newGroupMembers = new List<Guid> { user.Id, friend.Id };

        if (addFriendRequest.EncryptionKeys.Any(keyValuePair =>
                !newGroupMembers.Contains(keyValuePair.Key)))
        {
            return BadRequest();
        }

        databaseService.AddFriend(user, friend);

        var newGroup = databaseService.CreateGroup(user.DisplayName + "_" + friend.DisplayName, true);

        foreach (var memberId in newGroupMembers)
        {
            databaseService.AddUserToGroup(memberId, newGroup.Id);
            databaseService.AddGroupEncryptionKey(memberId, newGroup.Id,
                addFriendRequest.EncryptionKeys[memberId]);
        }

        return Ok();
    }

    [HttpGet("friendlist", Name = "GetFriendList")]
    public IActionResult FriendList([FromBody] FriendListRequestDto friendListRequest)
    {
        var user = databaseService.GetUserFromToken(friendListRequest.SessionToken);
        if (user == null) return BadRequest();

        var friends = user.Friends.Select(f => f.Id).ToList();

        return Ok(new GetFriendListResponseDto
        {
            Friends = friends
        });
    }

    [HttpGet("namefromguid", Name = "GetNameFromGuid")]
    public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest)
    {
        var user = databaseService.FindUserById(nameFromGuidRequest.Id);
        if (user == null) return BadRequest();

        return Ok(new NameFromGuidResponseDto
        {
            Name = user.DisplayName
        });
    }

    [HttpGet("usergroups", Name = "GetUserGroups")]
    public IActionResult UserGroups([FromBody] UserGroupsRequestDto userGroupsRequest)
    {
        var user = databaseService.GetUserFromToken(userGroupsRequest.SessionToken);
        if (user == null) return BadRequest();

        var groups = user.Groups.Select(g => g.Id).ToList();

        return Ok(new UserGroupsResponseDto
        {
            Groups = groups
        });
    }

    [HttpPost("userdisplayname", Name = "ChangeUserDisplayName")]
    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
    {
        var user = databaseService.GetUserFromToken(userDisplayNameRequest.Token);
        if (user == null) return BadRequest();

        databaseService.ChangeUserDisplayName(user, userDisplayNameRequest.Name);

        return Ok();
    }
}