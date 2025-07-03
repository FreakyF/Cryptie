using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Features.GroupManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Controllers;

[ApiController]
[Route("group")]
public class GroupManagementController(
    IGroupManagementService groupManagementService
) : ControllerBase
{
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
}