using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement.Services;

public class UserManagementService(IDatabaseService databaseService) : ControllerBase, IUserManagementService
{
    /// <summary>
    /// Resolves a user identifier from a valid session token.
    /// </summary>
    /// <param name="userGuidFromTokenRequest">Request containing the token.</param>
    /// <returns>User id when the token is valid.</returns>
    public IActionResult UserGuidFromToken([FromBody] UserGuidFromTokenRequestDto userGuidFromTokenRequest)
    {
        var user = databaseService.GetUserFromToken(userGuidFromTokenRequest.SessionToken);
        if (user == null) return BadRequest();

        return Ok(new UserGuidFromTokenResponseDto
        {
            Guid = user.Id
        });
    }

    /// <summary>
    /// Creates a private group for two users and stores provided encryption keys.
    /// </summary>
    /// <param name="addFriendRequest">Information about the friend and keys.</param>
    /// <returns>Result of the operation.</returns>
    public IActionResult AddFriend([FromBody] AddFriendRequestDto addFriendRequest)
    {
        var friend = databaseService.FindUserByLogin(addFriendRequest.Friend);
        if (friend == null) return NotFound();
        var user = databaseService.GetUserFromToken(addFriendRequest.SessionToken);
        if (user == null) return BadRequest();

        var newGroupMembers = new List<Guid> { user.Id, friend.Id };

        if (addFriendRequest.EncryptionKeys.Any(keyValuePair =>
                !newGroupMembers.Contains(keyValuePair.Key)))
            return BadRequest();

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

    /// <summary>
    /// Returns the display name of a user identified by GUID.
    /// </summary>
    /// <param name="nameFromGuidRequest">Request containing the user id.</param>
    /// <returns>User display name.</returns>
    public IActionResult NameFromGuid([FromBody] NameFromGuidRequestDto nameFromGuidRequest)
    {
        var user = databaseService.FindUserById(nameFromGuidRequest.Id);
        if (user == null) return BadRequest();

        return Ok(new NameFromGuidResponseDto
        {
            Name = user.DisplayName
        });
    }

    /// <summary>
    /// Returns identifiers of groups the user belongs to.
    /// </summary>
    /// <param name="userGroupsRequest">Request containing the session token.</param>
    /// <returns>List of group ids.</returns>
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

    /// <summary>
    /// Changes the display name for the authenticated user.
    /// </summary>
    /// <param name="userDisplayNameRequest">Request containing token and new name.</param>
    /// <returns>HTTP 200 on success.</returns>
    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
    {
        var user = databaseService.GetUserFromToken(userDisplayNameRequest.Token);
        if (user == null) return BadRequest();

        databaseService.ChangeUserDisplayName(user, userDisplayNameRequest.Name);

        return Ok();
    }

    /// <summary>
    /// Retrieves the private key and control value of a user.
    /// </summary>
    /// <param name="userPrivateKeyRequest">Request containing session token.</param>
    /// <returns>The private key pair.</returns>
    public IActionResult UserPrivateKey([FromBody] UserPrivateKeyRequestDto userPrivateKeyRequest)
    {
        var user = databaseService.GetUserFromToken(userPrivateKeyRequest.SessionToken);
        if (user == null) return Unauthorized();

        return Ok(new UserPrivateKeyResponseDto
        {
            PrivateKey = user.PrivateKey,
            ControlValue = user.ControlValue
        });
    }

    /// <summary>
    /// Finds a user identifier by login.
    /// </summary>
    /// <param name="guidFromLoginRequest">Request containing the login.</param>
    /// <returns>User id or 404.</returns>
    public IActionResult GuidFromLogin([FromBody] GuidFromLoginRequestDto guidFromLoginRequest)
    {
        var user = databaseService.FindUserByLogin(guidFromLoginRequest.Login);
        if (user == null) return NotFound();
        
        return Ok(new GuidFromLoginResponseDto
        {
            UserId = user.Id
        });
    }
}