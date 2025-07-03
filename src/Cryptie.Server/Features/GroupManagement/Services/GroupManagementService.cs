using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Services;

public class GroupManagementService(
    IDatabaseService databaseService
) : ControllerBase, IGroupManagementService
{
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