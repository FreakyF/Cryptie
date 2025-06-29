using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement;

public class GroupManagementService(
    IDatabaseService databaseService
) : ControllerBase, IGroupManagementService
{
    public IActionResult getName(GetGroupNameRequestDto getGroupNameRequest)
    {
        var group = databaseService.FindGroupById(getGroupNameRequest.GroupId);
        if (group == null) return NotFound();

        return Ok(new GetGroupNameResponseDto
        {
            name = group.Name
        });
    }

    public IActionResult createGroup(CreateGroupRequestDto createGroupRequest)
    {
        var user = databaseService.GetUserFromToken(createGroupRequest.SessionToken);
        if (user == null) return BadRequest();

        var group = databaseService.CreateNewGroup(user, createGroupRequest.Name);

        if (group == null) return BadRequest();

        return Ok(new CreateGroupResponseDto
        {
            Group = group.Id
        });
    }

    public IActionResult deleteGroup(DeleteGroupRequestDto deleteGroupRequest)
    {
        var user = databaseService.GetUserFromToken(deleteGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != deleteGroupRequest.GroupGuid)) return BadRequest();

        databaseService.DeleteGroup(deleteGroupRequest.GroupGuid);

        return Ok();
    }

    public IActionResult changeGroupName(ChangeGroupNameRequestDto changeGroupNameRequest)
    {
        var user = databaseService.GetUserFromToken(changeGroupNameRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != changeGroupNameRequest.GroupGuid)) return BadRequest();

        var group = databaseService.FindGroupById(changeGroupNameRequest.GroupGuid);
        if (group == null) return NotFound();

        databaseService.ChangeGroupName(changeGroupNameRequest.GroupGuid, changeGroupNameRequest.NewName);

        return Ok();
    }

    public IActionResult addUserToGroup(AddUserToGroupRequestDto addUserToGroupRequest)
    {
        var user = databaseService.GetUserFromToken(addUserToGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != addUserToGroupRequest.GroupGuid)) return BadRequest();

        databaseService.AddUserToGroup(addUserToGroupRequest.UserToAdd, addUserToGroupRequest.GroupGuid);

        return Ok();
    }

    public IActionResult removeUserFromGroup(RemoveUserFromGroupRequestDto removeUserFromGroupRequest)
    {
        var user = databaseService.GetUserFromToken(removeUserFromGroupRequest.SessionToken);
        if (user == null) return BadRequest();
        if (user.Groups.All(g => g.Id != removeUserFromGroupRequest.GroupGuid)) return BadRequest();

        databaseService.RemoveUserFromGroup(removeUserFromGroupRequest.UserToRemove,
            removeUserFromGroupRequest.GroupGuid);

        return Ok();
    }

    public IActionResult IsGroupPrivate(IsGroupPrivateRequestDto isGroupPrivateRequest)
    {
        var group = databaseService.FindGroupById(isGroupPrivateRequest.GroupId);
        if (group == null) return NotFound();
        return Ok(new IsGroupPrivateResponseDto { IsPrivate = group.IsPrivate });
    }

    public IActionResult IsGroupsPrivate(IsGroupsPrivateRequestDto isGroupsPrivateRequest)
    {
        var result = new Dictionary<Guid, bool>();
        foreach (var groupId in isGroupsPrivateRequest.GroupIds)
        {
            var group = databaseService.FindGroupById(groupId);
            if (group != null)
            {
                result[groupId] = group.IsPrivate;
            }
        }

        if (result.Count == 0) return NotFound();
        return Ok(new IsGroupsPrivateResponseDto { GroupStatuses = result });
    }

    public IActionResult GetGroupsNames(GetGroupsNamesRequestDto getGroupsNamesRequest)
    {
        var user = databaseService.GetUserFromToken(getGroupsNamesRequest.SessionToken);
        if (user == null) return Unauthorized();

        var result = user.Groups.ToDictionary(group => group.Id,
            group => group.IsPrivate ? group.Members.FirstOrDefault(m => m.Id != user.Id).DisplayName : group.Name);

        return Ok(new GetGroupsNamesResponseDto
        {
            GroupsNames = result
        });
    }
}