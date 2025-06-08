using Cryptie.Common.Features.UserManagement.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Features.UserManagement.Services;

namespace Server.Features.UserManagement;

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