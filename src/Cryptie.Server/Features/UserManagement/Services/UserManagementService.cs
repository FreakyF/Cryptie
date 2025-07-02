using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement.Services;

public class UserManagementService(IDatabaseService databaseService) : ControllerBase, IUserManagementService
{
    public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest)
    {
        var user = databaseService.GetUserFromToken(userGuidFromTokenRequest.SessionToken);
        if (user == null) return BadRequest();

        return Ok(new UserGuidFromTokenResponseDto
        {
            Guid = user.Id
        });
    }

    public IActionResult UserLoginFromToken([FromBody] UserLoginFromTokenRequestDto userLoginFromTokenRequest)
    {
        var user = databaseService.GetUserFromToken(userLoginFromTokenRequest.SessionToken);
        if (user == null) return BadRequest();

        return Ok(new UserLoginFromTokenResponseDto
        {
            Login = user.Login
        });
    }

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

    public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest)
    {
        var user = databaseService.FindUserById(nameFromGuidRequest.Id);
        if (user == null) return BadRequest();

        return Ok(new NameFromGuidResponseDto
        {
            Name = user.DisplayName
        });
    }

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

    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
    {
        var user = databaseService.GetUserFromToken(userDisplayNameRequest.Token);
        if (user == null) return BadRequest();

        databaseService.ChangeUserDisplayName(user, userDisplayNameRequest.Name);

        return Ok();
    }
}