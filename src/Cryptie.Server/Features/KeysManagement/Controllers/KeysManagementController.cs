using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Features.KeysManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement.Controllers;

[ApiController]
[Route("keys")]
public class KeysManagementController(IKeysManagementService keysManagementService) : ControllerBase
{
    [HttpGet("user")]
    public IActionResult getUserKey([FromBody] GetUserKeyRequestDto getUserKeyRequest)
    {
        return keysManagementService.getUserKey(getUserKeyRequest);
    }

    [HttpGet("groupsKey", Name = "GetGroupsKey")]
    public IActionResult getGroupKey([FromBody] GetGroupsKeyRequestDto getGroupsKeyRequest)
    {
        return keysManagementService.getGroupsKey(getGroupsKeyRequest);
    }
}