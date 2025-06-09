using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Features.UserManagement.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement;

[ApiController]
[Route("user")]
public class UserManagementController(DatabaseService databaseService) : ControllerBase
{
    [HttpPost("user", Name = "GetUser")]
    public IActionResult User([FromBody] UserRequestDto userRequest)
    {
        var user = databaseService.GetUserFromToken(userRequest.Toekn);
        return Ok(new UserResponseDto { User = user });
    }
}