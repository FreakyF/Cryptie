using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement.Services;

public class UserManagementService(IDatabaseService databaseService) : ControllerBase, IUserManagementService
{
    /// <summary>
    ///     Returns user GUID associated with a session token.
    /// </summary>
    /// <param name="userGuidFromTokenRequest">Request containing session token.</param>
    /// <returns>User identifier on success.</returns>
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
    ///     Adds a user to the current user's friend list and creates a private group for them.
    /// </summary>
    /// <param name="addFriendRequest">Request containing friend login and encryption keys.</param>
    /// <returns><see cref="OkResult"/> when the friend was added.</returns>
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
    ///     Retrieves the display name of a user by their GUID.
    /// </summary>
    /// <param name="nameFromGuidRequest">Request with the user's id.</param>
    /// <returns>Display name of the user.</returns>
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
    ///     Returns identifiers of groups the user belongs to.
    /// </summary>
    /// <param name="userGroupsRequest">Request containing session token.</param>
    /// <returns>List of group identifiers.</returns>
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
    ///     Changes the display name of the authenticated user.
    /// </summary>
    /// <param name="userDisplayNameRequest">Request containing token and new name.</param>
    /// <returns><see cref="OkResult"/> when the change was applied.</returns>
    public IActionResult UserDisplayName([FromBody] UserDisplayNameRequestDto userDisplayNameRequest)
    {
        var user = databaseService.GetUserFromToken(userDisplayNameRequest.Token);
        if (user == null) return BadRequest();

        databaseService.ChangeUserDisplayName(user, userDisplayNameRequest.Name);

        return Ok();
    }

    /// <summary>
    ///     Returns the user's private key used for end-to-end encryption.
    /// </summary>
    /// <param name="userPrivateKeyRequest">Request containing session token.</param>
    /// <returns>Private key and associated control value.</returns>
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
    ///     Retrieves the unique identifier of a user from their login name.
    /// </summary>
    /// <param name="guidFromLoginRequest">Request containing user login.</param>
    /// <returns>User identifier when found.</returns>
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