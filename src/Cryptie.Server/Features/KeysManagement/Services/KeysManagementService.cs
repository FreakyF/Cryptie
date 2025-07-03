using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Services;

public class KeysManagementService(IDatabaseService databaseService) : ControllerBase, IKeysManagementService
{
    /// <summary>
    /// Retrieves the public key of a specific user.
    /// </summary>
    /// <param name="getUserKeyRequest">Request containing the user identifier.</param>
    /// <returns>The user's public key.</returns>
    public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest)
    {
        var key = databaseService.GetUserPublicKey(getUserKeyRequest.UserId);
        return Ok(new GetUserKeyResponseDto
        {
            PublicKey = key
        });
    }

    /// <summary>
    /// Gets encryption keys for all groups the requesting user is part of.
    /// </summary>
    /// <param name="getGroupsKeyRequest">Request containing the session token.</param>
    /// <returns>Dictionary of group ids with their encryption keys.</returns>
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