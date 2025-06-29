using Cryptie.Common.Features.GroupManagement;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement;

public interface IGroupManagementService
{
    public IActionResult getName(GetGroupNameRequestDto getGroupNameRequest);

    public IActionResult createGroup(CreateGroupRequestDto createGroupRequest);

    public IActionResult deleteGroup(DeleteGroupRequestDto deleteGroupRequest);

    public IActionResult changeGroupName(ChangeGroupNameRequestDto changeGroupNameRequest);

    public IActionResult addUserToGroup(AddUserToGroupRequestDto addUserToGroupRequest);

    public IActionResult removeUserFromGroup(RemoveUserFromGroupRequestDto removeUserFromGroupRequest);

    public IActionResult IsGroupPrivate(IsGroupPrivateRequestDto isGroupPrivateRequest);
    public IActionResult IsGroupsPrivate(IsGroupsPrivateRequestDto isGroupsPrivateRequest);
    public IActionResult GetGroupsNames(GetGroupsNamesRequestDto getGroupsNamesRequest);
}