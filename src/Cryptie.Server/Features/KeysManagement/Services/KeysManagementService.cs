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
    
    public IActionResult saveUserKeys([FromBody] SaveUserKeysRequestDto saveUserKeysRequest)
    {
        var user = databaseService.GetUserFromToken(saveUserKeysRequest.userToken);
        if (user == null)
        {
            return Unauthorized();
        }

        databaseService.SaveUserKeys(user, saveUserKeysRequest.privateKey, saveUserKeysRequest.publicKey);

        return Ok();
    }
}