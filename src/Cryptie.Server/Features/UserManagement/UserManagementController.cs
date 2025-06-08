using Cryptie.Server.Features.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.UserManagement;

[ApiController]
[Route("user")]
public class UserManagementController(DatabaseService databaseService)
{
    // [HttpPost("user", Name = "GetUser")]
    // public IActionResult User([FromBody] UserRequestDto userRequest)
    // {
    //     return Ok("a");
    // }
}