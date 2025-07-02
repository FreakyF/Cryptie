using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Controllers;

[ApiController]
[Route("group")]
public class GroupManagementController(
    IGroupManagementService groupManagementService
) : ControllerBase
{
    [HttpPost("changeGroupName", Name = "ChangeGroupName")]
    public IActionResult changeGroupName([FromBody] ChangeGroupNameRequestDto changeGroupNameRequest)
    {
        return groupManagementService.changeGroupName(changeGroupNameRequest);
    }

    [HttpPost("add", Name = "AddUserToGroup")]
    public IActionResult addUserToGroup([FromBody] AddUserToGroupRequestDto addUserToGroupRequest)
    {
        return groupManagementService.addUserToGroup(addUserToGroupRequest);
    }

    [HttpPost("isGroupsPrivate", Name = "IsGroupsPrivate")]
    public IActionResult IsGroupsPrivate([FromBody] IsGroupsPrivateRequestDto isGroupsPrivateRequest)
    {
        return groupManagementService.IsGroupsPrivate(isGroupsPrivateRequest);
    }

    [HttpPost("groupsNames", Name = "GetGroupsNames")]
    public IActionResult IsGroupsPrivate([FromBody] GetGroupsNamesRequestDto getGroupsNamesRequest)
    {
        return groupManagementService.GetGroupsNames(getGroupsNamesRequest);
    }

    [HttpPost("createGroupFromPrivateChat", Name = "CreateGroupFromPrivateChat")]
    public IActionResult CreateGroupFromPrivateChat(
        [FromBody] CreateGroupFromPrivateChatRequestDto createGroupFromPrivateChatRequest)
    {
        return groupManagementService.CreateGroupFromPrivateChat(createGroupFromPrivateChatRequest);
    }
}