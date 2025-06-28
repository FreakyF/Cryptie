using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement;

[ApiController]
[Route("group")]
public class GroupManagementController(
    IGroupManagementService groupManagementService
) : ControllerBase
{
    [HttpGet("getName", Name = "GetGroupName")]
    public IActionResult getName([FromBody] GetGroupNameRequestDto getGroupNameRequest)
    {
        return groupManagementService.getName(getGroupNameRequest);
    }

    [HttpPost("create", Name = "CreateGroup")]
    public IActionResult createGroup([FromBody] CreateGroupRequestDto createGroupRequest)
    {
        return groupManagementService.createGroup(createGroupRequest);
    }

    [HttpDelete("delete", Name = "DeleteGroup")]
    public IActionResult deleteGroup([FromBody] DeleteGroupRequestDto deleteGroupRequest)
    {
        return groupManagementService.deleteGroup(deleteGroupRequest);
    }

    [HttpPost("name", Name = "ChangeGroupName")]
    public IActionResult changeGroupName([FromBody] ChangeGroupNameRequestDto changeGroupNameRequest)
    {
        return groupManagementService.changeGroupName(changeGroupNameRequest);
    }

    [HttpPost("add", Name = "AddUserToGroup")]
    public IActionResult addUserToGroup([FromBody] AddUserToGroupRequestDto addUserToGroupRequest)
    {
        return groupManagementService.addUserToGroup(addUserToGroupRequest);
    }

    [HttpPost("remove", Name = "RemoveUserFromGroup")]
    public IActionResult removeUserFromGroup([FromBody] RemoveUserFromGroupRequestDto removeUserFromGroupRequest)
    {
        return groupManagementService.removeUserFromGroup(removeUserFromGroupRequest);
    }

    [HttpPost("isPrivate", Name = "IsGroupPrivate")]
    public IActionResult IsGroupPrivate([FromBody] IsGroupPrivateRequestDto isGroupPrivateRequest)
    {
        return groupManagementService.IsGroupPrivate(isGroupPrivateRequest);
    }
}