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
/// <summary>
/// Determines whether private groups are enabled on the server.
/// </summary>
/// <param name="isGroupsPrivateRequest">Request containing session token.</param>
/// <returns>True if private groups are enabled.</returns>
public IActionResult IsGroupsPrivate([FromBody] IsGroupsPrivateRequestDto isGroupsPrivateRequest)
{
    return groupManagementService.IsGroupsPrivate(isGroupsPrivateRequest);
}

[HttpPost("groupsNames", Name = "GetGroupsNames")]
/// <summary>
/// Returns display names of groups specified by their identifiers.
/// </summary>
/// <param name="getGroupsNamesRequest">Request with group ids.</param>
/// <returns>List of group names.</returns>
public IActionResult IsGroupsPrivate([FromBody] GetGroupsNamesRequestDto getGroupsNamesRequest)
{
    return groupManagementService.GetGroupsNames(getGroupsNamesRequest);
}
}