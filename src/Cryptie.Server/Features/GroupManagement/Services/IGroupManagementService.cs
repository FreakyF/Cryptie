using Cryptie.Common.Features.GroupManagement;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement;

public interface IGroupManagementService
{
    public IActionResult changeGroupName(ChangeGroupNameRequestDto changeGroupNameRequest);

    public IActionResult addUserToGroup(AddUserToGroupRequestDto addUserToGroupRequest);
    public IActionResult IsGroupsPrivate(IsGroupsPrivateRequestDto isGroupsPrivateRequest);
    public IActionResult GetGroupsNames(GetGroupsNamesRequestDto getGroupsNamesRequest);

    public IActionResult CreateGroupFromPrivateChat(
        CreateGroupFromPrivateChatRequestDto createGroupFromPrivateChatRequest);
}