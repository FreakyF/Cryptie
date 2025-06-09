using Cryptie.Common.Entities.Group;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Features.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement;

[ApiController]
[Route("user")]
public class UserManagementController(DatabaseService databaseService) : ControllerBase
{
    [HttpGet("user", Name = "GetUser")]
    public IActionResult User([FromBody] UserRequestDto userRequest)
    {
        var user = databaseService.GetUserFromToken(userRequest.Toekn);
        return Ok(new UserResponseDto { User = user });
    }

    [HttpPost("addfriend", Name = "PostAddFriend")]
    public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest)
    {
        var user = databaseService.GetUserFromToken(addFriendRequest.Token);
        if (user == null) return BadRequest();
        var friend = databaseService.FindUserById(addFriendRequest.Friend);
        if (friend == null) return BadRequest();
        user.Friends.Add(friend);
        // friend.Friends.Add(user); // TODO może ???

        var group = new Group
        {
            Name = user.DisplayName + "_" + friend.DisplayName
        };
        
        group.Users.Add(user);
        group.Users.Add(friend);
        
        return Ok();
    }

    [HttpGet("friendlist", Name = "GetFriendList")]
    public IActionResult FriendList([FromBody] FriendListRequestDto friendListRequest)
    {
        var user = databaseService.GetUserFromToken(friendListRequest.Toekn);
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
}