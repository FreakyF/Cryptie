using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Features.KeysManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Controllers;

[ApiController]
[Route("keys")]
public class KeysManagementController(IKeysManagementService keysManagementService) : ControllerBase
{
[HttpGet("user")]
/// <summary>
/// Retrieves the symmetric key for the specified user.
/// </summary>
/// <param name="getUserKeyRequest">Request describing the user.</param>
/// <returns>The encryption key if found.</returns>
public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest)
{
    return keysManagementService.getUserKey(getUserKeyRequest);
}

[HttpGet("groupsKey", Name = "GetGroupsKey")]
/// <summary>
/// Returns the encryption key used for a group conversation.
/// </summary>
/// <param name="getGroupsKeyRequest">Information about the group.</param>
/// <returns>Key material for the group.</returns>
public IActionResult getGroupKey([FromBody] GetGroupsKeyRequestDto getGroupsKeyRequest)
{
    return keysManagementService.getGroupsKey(getGroupsKeyRequest);
}
}