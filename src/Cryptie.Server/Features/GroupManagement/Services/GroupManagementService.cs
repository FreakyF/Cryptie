using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.GroupManagement.Services;

public class GroupManagementService(
    IDatabaseService databaseService
) : ControllerBase, IGroupManagementService
{
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

        var userToAdd = databaseService.FindUserById(addUserToGroupRequest.UserToAdd);
        if (userToAdd == null) return NotFound();

        var group = databaseService.FindGroupById(addUserToGroupRequest.GroupGuid);
        if (group == null) return NotFound();

        if (user.Groups.All(g => g.Id != addUserToGroupRequest.GroupGuid)) return BadRequest();

        databaseService.AddUserToGroup(addUserToGroupRequest.UserToAdd, addUserToGroupRequest.GroupGuid);
        databaseService.AddGroupEncryptionKey(userToAdd.Id, group.Id, addUserToGroupRequest.EncryptionKey);

        return Ok();
    }

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

    public IActionResult CreateGroupFromPrivateChat(
        CreateGroupFromPrivateChatRequestDto createGroupFromPrivateChatRequest)
    {
        var user = databaseService.GetUserFromToken(createGroupFromPrivateChatRequest.SessionToken);
        if (user == null) return Unauthorized();

        var privateChat = databaseService.FindGroupById(createGroupFromPrivateChatRequest.PrivateChatId);
        if (privateChat is not { IsPrivate: true })
            return BadRequest();

        if (privateChat.Members.All(m => m.Id != user.Id))
            return Unauthorized();

        var newMember = databaseService.FindUserByLogin(createGroupFromPrivateChatRequest.NewMember);
        if (newMember == null)
            return NotFound();

        if (privateChat.Members.All(m => m.Id != newMember.Id))
            return BadRequest();

        var newGroupMembers = privateChat.Members.Select(m => m.Id).ToList();
        newGroupMembers.Add(newMember.Id);

        if (createGroupFromPrivateChatRequest.EncryptionKeys.Any(keyValuePair =>
                !newGroupMembers.Contains(keyValuePair.Key)))
            return BadRequest();

        var newGroup = databaseService.CreateNewGroup(user, privateChat.Name + "_" + newMember.DisplayName);
        if (newGroup == null) return BadRequest();

        foreach (var memberId in newGroupMembers)
        {
            databaseService.AddUserToGroup(memberId, newGroup.Id);
            databaseService.AddGroupEncryptionKey(memberId, newGroup.Id,
                createGroupFromPrivateChatRequest.EncryptionKeys[memberId]);
        }

        return Ok(new CreateGroupFromPrivateChatResponseDto
        {
            GroupId = newGroup.Id
        });
    }
}