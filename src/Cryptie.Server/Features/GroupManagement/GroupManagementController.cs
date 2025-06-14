using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagment;

[ApiController]
[Route("group")]
public class GroupManagementController(
    IDatabaseService databaseService
) : ControllerBase
{
    [HttpPost("create", Name = "CreateGroup")]
    public IActionResult createGroup([FromBody] CreateGroupRequestDto createGroupRequest)
    {
        var user = databaseService.GetUserFromToken(createGroupRequest.SessionToken);
        if (user == null) return BadRequest();

        var group = databaseService.CreateNewGroup(user, createGroupRequest.Name);

        if (group == null)
        {
            return BadRequest();
        }

        return Ok(new CreateGroupResponseDto
        {
            Group = group.Id,
        });
    }

    [HttpDelete("delete", Name = "DeleteGroup")]
    public IActionResult deleteGroup([FromBody] DeleteGroupRequestDto deleteGroupRequest)
    {
        var user = databaseService.GetUserFromToken(deleteGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != deleteGroupRequest.GroupGuid)) return BadRequest();

        databaseService.DeleteGroup(deleteGroupRequest.GroupGuid);

        return Ok();
    }

    [HttpPost("name", Name = "ChangeGroupName")]
    public IActionResult changeGroupName([FromBody] ChangeGroupNameRequestDto changeGroupNameRequest)
    {
        var user = databaseService.GetUserFromToken(changeGroupNameRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != changeGroupNameRequest.GroupGuid)) return BadRequest();

        databaseService.ChangeGroupName(changeGroupNameRequest.GroupGuid, changeGroupNameRequest.NewName);

        return Ok();
    }

    [HttpPost("add", Name = "AddUserToGroup")]
    public IActionResult addUserToGroup([FromBody] AddUserToGroupRequestDto addUserToGroupRequest)
    {
        var user = databaseService.GetUserFromToken(addUserToGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != addUserToGroupRequest.GroupGuid)) return BadRequest();

        databaseService.AddUserToGroup(addUserToGroupRequest.UserToAdd, addUserToGroupRequest.GroupGuid);

        return Ok();
    }

    [HttpPost("remove", Name = "RemoveUserFromGroup")]
    public IActionResult removeUserFromGroup([FromBody] RemoveUserFromGroupRequestDto removeUserFromGroupRequest)
    {
        var user = databaseService.GetUserFromToken(removeUserFromGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != removeUserFromGroupRequest.GroupGuid)) return BadRequest();

        databaseService.RemoveUserFromGroup(removeUserFromGroupRequest.UserToRemove,
            removeUserFromGroupRequest.GroupGuid);

        return Ok();
    }
}