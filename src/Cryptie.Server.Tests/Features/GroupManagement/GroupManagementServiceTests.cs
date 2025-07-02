using Cryptie.Common.Entities;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cryptie.Server.Tests.Features.GroupManagement;

public class GroupManagementServiceTests
{
    private readonly Mock<IDatabaseService> _dbMock;
    private readonly GroupManagementService _service;

    public GroupManagementServiceTests()
    {
        _dbMock = new Mock<IDatabaseService>();
        _service = new GroupManagementService(_dbMock.Object);
    }

    [Fact]
    public void ChangeGroupName_ReturnsOk_WhenUserOwnsGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group> { new Group { Id = groupId } } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(groupId)).Returns(new Group { Id = groupId, Name = "Old" });
        var result = _service.changeGroupName(new ChangeGroupNameRequestDto { SessionToken = userId, GroupGuid = groupId, NewName = "New" });
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void ChangeGroupName_ReturnsBadRequest_WhenUserIsNull()
    {
        var badToken = Guid.NewGuid();
        _dbMock.Setup(x => x.GetUserFromToken(badToken)).Returns((User)null);
        var result = _service.changeGroupName(new ChangeGroupNameRequestDto { SessionToken = badToken, GroupGuid = Guid.NewGuid(), NewName = "New" });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void ChangeGroupName_ReturnsBadRequest_WhenUserNotInGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group>() };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.changeGroupName(new ChangeGroupNameRequestDto { SessionToken = userId, GroupGuid = groupId, NewName = "New" });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void ChangeGroupName_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group> { new Group { Id = groupId } } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(groupId)).Returns((Group)null);
        var result = _service.changeGroupName(new ChangeGroupNameRequestDto { SessionToken = userId, GroupGuid = groupId, NewName = "New" });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void AddUserToGroup_ReturnsOk_WhenUserOwnsGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userToAdd = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group> { new Group { Id = groupId } } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.FindUserById(userToAdd)).Returns(new User { Id = userToAdd });
        _dbMock.Setup(x => x.FindGroupById(groupId)).Returns(new Group { Id = groupId });
        var result = _service.addUserToGroup(new AddUserToGroupRequestDto { SessionToken = userId, GroupGuid = groupId, UserToAdd = userToAdd });
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(x => x.AddUserToGroup(userToAdd, groupId), Times.Once);
    }

    [Fact]
    public void AddUserToGroup_ReturnsBadRequest_WhenUserIsNull()
    {
        var badToken = Guid.NewGuid();
        _dbMock.Setup(x => x.GetUserFromToken(badToken)).Returns((User)null);
        var result = _service.addUserToGroup(new AddUserToGroupRequestDto { SessionToken = badToken, GroupGuid = Guid.NewGuid(), UserToAdd = Guid.NewGuid() });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void AddUserToGroup_ReturnsBadRequest_WhenUserNotInGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group>() };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.addUserToGroup(new AddUserToGroupRequestDto { SessionToken = userId, GroupGuid = groupId, UserToAdd = Guid.NewGuid() });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetGroupsNames_ReturnsUnauthorized_WhenUserIsNull()
    {
        _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
        var result = _service.GetGroupsNames(new GetGroupsNamesRequestDto { SessionToken = Guid.NewGuid() });
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetGroupsNames_ReturnsOk_WithPublicAndPrivateGroups()
    {
        var userId = Guid.NewGuid();
        var publicGroup = new Group { Id = Guid.NewGuid(), Name = "Public", IsPrivate = false };
        var privateGroup = new Group { Id = Guid.NewGuid(), Name = "Private", IsPrivate = true, Members = new List<User> { new User { Id = userId }, new User { Id = Guid.NewGuid(), DisplayName = "OtherUser" } } };
        var user = new User { Id = userId, Groups = new List<Group> { publicGroup, privateGroup } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(publicGroup.Id)).Returns(publicGroup);
        _dbMock.Setup(x => x.FindGroupById(privateGroup.Id)).Returns(privateGroup);
        _dbMock.Setup(x => x.FindUserById(It.IsAny<Guid>())).Returns<Guid>(id => new User { Id = id, DisplayName = "OtherUser" });
        var result = _service.GetGroupsNames(new GetGroupsNamesRequestDto { SessionToken = userId });
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<GetGroupsNamesResponseDto>(ok.Value);
        Assert.Contains(publicGroup.Id, dto.GroupsNames.Keys);
        Assert.Contains(privateGroup.Id, dto.GroupsNames.Keys);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsUnauthorized_WhenUserIsNull()
    {
        _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
        var result = _service.CreateGroupFromPrivateChat(new CreateGroupFromPrivateChatRequestDto { SessionToken = Guid.NewGuid() });
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsBadRequest_WhenPrivateChatIsNullOrNotPrivate()
    {
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns((Group)null);
        var req = new CreateGroupFromPrivateChatRequestDto { SessionToken = user.Id, PrivateChatId = Guid.NewGuid() };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<BadRequestResult>(result);
        // not private
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(new Group { IsPrivate = false });
        result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsUnauthorized_WhenUserNotInPrivateChat()
    {
        var user = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { IsPrivate = true, Members = new List<User> { new User { Id = Guid.NewGuid() } } };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(privateChat);
        var req = new CreateGroupFromPrivateChatRequestDto { SessionToken = user.Id, PrivateChatId = Guid.NewGuid() };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsNotFound_WhenNewMemberNotFound()
    {
        var user = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { IsPrivate = true, Members = new List<User> { user } };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(privateChat);
        _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns((User)null);
        var req = new CreateGroupFromPrivateChatRequestDto { SessionToken = user.Id, PrivateChatId = Guid.NewGuid(), NewMember = "login" };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsBadRequest_WhenNewMemberNotInPrivateChat()
    {
        var user = new User { Id = Guid.NewGuid() };
        var newMember = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { IsPrivate = true, Members = new List<User> { user } };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(privateChat);
        _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(newMember);
        var req = new CreateGroupFromPrivateChatRequestDto { SessionToken = user.Id, PrivateChatId = Guid.NewGuid(), NewMember = "login" };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsBadRequest_WhenEncryptionKeysMissing()
    {
        var user = new User { Id = Guid.NewGuid() };
        var newMember = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { IsPrivate = true, Members = new List<User> { user, newMember } };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(privateChat);
        _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(newMember);
        var req = new CreateGroupFromPrivateChatRequestDto {
            SessionToken = user.Id,
            PrivateChatId = Guid.NewGuid(),
            NewMember = "login",
            EncryptionKeys = new Dictionary<Guid, string>() // brak kluczy
        };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsBadRequest_WhenCreateNewGroupFails()
    {
        var user = new User { Id = Guid.NewGuid() };
        var newMember = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { IsPrivate = true, Members = new List<User> { user, newMember } };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns(privateChat);
        _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(newMember);
        _dbMock.Setup(x => x.CreateNewGroup(user, It.IsAny<string>())).Returns((Group)null);
        var req = new CreateGroupFromPrivateChatRequestDto {
            SessionToken = user.Id,
            PrivateChatId = Guid.NewGuid(),
            NewMember = "login",
            EncryptionKeys = new Dictionary<Guid, string> { { user.Id, "key1" }, { newMember.Id, "key2" } }
        };
        var result = _service.CreateGroupFromPrivateChat(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void CreateGroupFromPrivateChat_ReturnsOk_WhenSuccess()
    {
        var user = new User { Id = Guid.NewGuid() };
        var newMember = new User { Id = Guid.NewGuid() };
        var privateChat = new Group { Id = Guid.NewGuid(), Name = "chat", IsPrivate = true, Members = new List<User> { user, newMember } };
        var newGroup = new Group { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(user.Id)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(privateChat.Id)).Returns(privateChat);
        _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(newMember);
        _dbMock.Setup(x => x.CreateNewGroup(user, It.IsAny<string>())).Returns(newGroup);
        var req = new CreateGroupFromPrivateChatRequestDto {
            SessionToken = user.Id,
            PrivateChatId = privateChat.Id,
            NewMember = "login",
            EncryptionKeys = new Dictionary<Guid, string> { { user.Id, "key1" }, { newMember.Id, "key2" } }
        };
        var result = _service.CreateGroupFromPrivateChat(req);
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CreateGroupFromPrivateChatResponseDto>(ok.Value);
        Assert.Equal(newGroup.Id, dto.GroupId);
    }
}
