using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Services;

public class GroupManagementService(
    IDatabaseService databaseService
) : ControllerBase, IGroupManagementService
{
    /// <summary>
    /// Returns privacy flags for the requested groups.
    /// </summary>
    /// <param name="isGroupsPrivateRequest">Request containing group identifiers.</param>
    /// <returns>Dictionary of group ids with their privacy status.</returns>
    public IActionResult IsGroupsPrivate(IsGroupsPrivateRequestDto isGroupsPrivateRequest)
    {
        var result = new Dictionary<Guid, bool>();
        foreach (var groupId in isGroupsPrivateRequest.GroupIds)
        {
            var group = databaseService.FindGroupById(groupId);
            if (group != null) result[groupId] = group.IsPrivate;
        }

        if (result.Count == 0) return NotFound();
        return Ok(new IsGroupsPrivateResponseDto { GroupStatuses = result });
    }

    /// <summary>
    /// Gets display names for all groups that a user is a member of.
    /// </summary>
    /// <param name="getGroupsNamesRequest">Request including the session token.</param>
    /// <returns>Mapping of group ids to display names.</returns>
    public IActionResult GetGroupsNames([FromBody] GetGroupsNamesRequestDto getGroupsNamesRequest)
    {
        var user = databaseService.GetUserFromToken(getGroupsNamesRequest.SessionToken);
        if (user == null)
            return Unauthorized();

        var result = new Dictionary<Guid, string>();

        foreach (var g in user.Groups)
        {
            var fullGroup = databaseService.FindGroupById(g.Id);
            if (fullGroup == null)
                continue;

            if (!fullGroup.IsPrivate)
            {
                result[fullGroup.Id] = fullGroup.Name;
            }
            else
            {
                var otherMember = fullGroup.Members
                    .FirstOrDefault(m => m.Id != user.Id);

                if (otherMember != null)
                {
                    var otherUser = databaseService.FindUserById(otherMember.Id);
                    result[fullGroup.Id] = otherUser?.DisplayName
                                           ?? otherMember.DisplayName;
                }
                else
                {
                    result[fullGroup.Id] = fullGroup.Name;
                }
            }
        }

        return Ok(new GetGroupsNamesResponseDto
        {
            GroupsNames = result
        });
    }
}