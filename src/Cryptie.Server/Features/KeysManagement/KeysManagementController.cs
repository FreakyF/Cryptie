using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.KeysManagement;

[ApiController]
[Route("keys")]
public class KeysManagementController(IDatabaseService databaseService) : ControllerBase
{
    [HttpGet("user")]
    public IActionResult getUserKey(GetUserKeyRequestDto userKeyRequest)
    {
        return Ok();
    }
}