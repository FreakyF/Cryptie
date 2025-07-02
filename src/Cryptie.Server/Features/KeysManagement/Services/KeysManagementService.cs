using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Services;

public class KeysManagementService(IDatabaseService databaseService) : ControllerBase, IKeysManagementService
{
    public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest)
    {
        var key = databaseService.GetUserPublicKey(getUserKeyRequest.UserId);
        return Ok(new GetUserKeyResponseDto
        {
            PublicKey = key
        });
    }

    public IActionResult getGroupKey([FromBody] GetGroupKeyRequestDto getGroupKeyRequest)
    {
        var user = databaseService.GetUserFromToken(getGroupKeyRequest.SessionToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var group = databaseService.FindGroupById(getGroupKeyRequest.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        if (user.Groups.All(g => g.Id != getGroupKeyRequest.GroupId)) return BadRequest();

        var key = databaseService.getGroupEncryptionKey(user.Id, group.Id);

        return Ok(new GetGroupKeyResponseDto
        {
            AesKey = key
        });
    }
}