using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Services;

public class KeysManagementService(IDatabaseService databaseService) : ControllerBase, IKeysManagementService
{
    /// <summary>
    ///     Retrieves public key of specified user.
    /// </summary>
    /// <param name="getUserKeyRequest">Request containing the user identifier.</param>
    /// <returns>DTO with the public key string.</returns>
    public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest)
    {
        var key = databaseService.GetUserPublicKey(getUserKeyRequest.UserId);
        return Ok(new GetUserKeyResponseDto
        {
            PublicKey = key
        });
    }

    /// <summary>
    ///     Retrieves AES keys for all groups the current user belongs to.
    /// </summary>
    /// <param name="getGroupsKeyRequest">Request containing the session token.</param>
    /// <returns>Dictionary mapping group ids to their AES keys.</returns>
    public IActionResult getGroupsKey([FromBody] GetGroupsKeyRequestDto getGroupsKeyRequest)
    {
        var user = databaseService.GetUserFromToken(getGroupsKeyRequest.SessionToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = new Dictionary<Guid, string>();

        foreach (var g in user.Groups)
        {
            var fullGroup = databaseService.FindGroupById(g.Id);

            if (fullGroup == null)
                continue;

            result[fullGroup.Id] = databaseService.getGroupEncryptionKey(user.Id, fullGroup.Id);
        }

        return Ok(new GetGroupsKeyResponseDto
        {
            Keys = result
        });
    }
}