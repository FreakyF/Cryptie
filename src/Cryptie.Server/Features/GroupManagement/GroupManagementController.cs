using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagment.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagment;

[ApiController]
[Route("group")]
public class GroupManagementController(DatabaseService databaseService) : ControllerBase
{
    [HttpPost("create", Name = "CreateGroup")]
    public IActionResult createGroup([FromBody] CreateGroupRequestDTO createGroupRequest)
    {
        var user = databaseService.GetUserFromToken(createGroupRequest.Token);
        if (user == null) return BadRequest();

        var group = databaseService.CreateNewGroup(user, createGroupRequest.Name);

        if (group == null)
        {
            return BadRequest();
        }
        
        return Ok(new CreateGroupResponseDTO
        {
            Group = group.Id,
        });
    }

    [HttpPost("add", Name="AddUserToGroup")]
    public IActionResult addUserToGroup([FromBody] AddUserToGroupRequestDTO addUserToGroupRequest)
    {
        var user = databaseService.GetUserFromToken(addUserToGroupRequest.Token);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != addUserToGroupRequest.GroupGuid)) return BadRequest();

        databaseService.AddUserToGroup(addUserToGroupRequest.UserToAdd, addUserToGroupRequest.GroupGuid);
        
        return Ok();
    }
}