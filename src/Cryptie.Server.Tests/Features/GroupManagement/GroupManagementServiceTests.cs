using Cryptie.Common.Entities;
using Cryptie.Common.Features.GroupManagement;
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
    public void GetName_ReturnsOk_WhenGroupExists()
    {
        var groupId = Guid.NewGuid();
        _dbMock.Setup(x => x.FindGroupById(groupId)).Returns(new Group { Id = groupId, Name = "TestGroup" });
        var result = _service.getName(new GetGroupNameRequestDto { GroupId = groupId });
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<GetGroupNameResponseDto>(ok.Value);
        Assert.Equal("TestGroup", dto.name);
    }

    [Fact]
    public void GetName_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns((Group)null);
        var result = _service.getName(new GetGroupNameRequestDto { GroupId = Guid.NewGuid() });
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void CreateGroup_ReturnsOk_WhenUserAndGroupCreated()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        var group = new Group { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.CreateNewGroup(user, "group")).Returns(group);
        var result = _service.createGroup(new CreateGroupRequestDto { SessionToken = userId, Name = "group" });
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<CreateGroupResponseDto>(ok.Value);
        Assert.Equal(group.Id, dto.Group);
    }

    [Fact]
    public void CreateGroup_ReturnsBadRequest_WhenUserIsNull()
    {
        var badToken = Guid.NewGuid();
        _dbMock.Setup(x => x.GetUserFromToken(badToken)).Returns((User)null);
        var result = _service.createGroup(new CreateGroupRequestDto { SessionToken = badToken, Name = "group" });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void CreateGroup_ReturnsBadRequest_WhenGroupIsNull()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.CreateNewGroup(user, "group")).Returns((Group)null);
        var result = _service.createGroup(new CreateGroupRequestDto { SessionToken = userId, Name = "group" });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void DeleteGroup_ReturnsOk_WhenUserOwnsGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group> { new Group { Id = groupId } } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.deleteGroup(new DeleteGroupRequestDto { SessionToken = userId, GroupGuid = groupId });
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(x => x.DeleteGroup(groupId), Times.Once);
    }

    [Fact]
    public void DeleteGroup_ReturnsBadRequest_WhenUserIsNull()
    {
        var badToken = Guid.NewGuid();
        _dbMock.Setup(x => x.GetUserFromToken(badToken)).Returns((User)null);
        var result = _service.deleteGroup(new DeleteGroupRequestDto { SessionToken = badToken, GroupGuid = Guid.NewGuid() });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void DeleteGroup_ReturnsBadRequest_WhenUserNotInGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group>() };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.deleteGroup(new DeleteGroupRequestDto { SessionToken = userId, GroupGuid = groupId });
        Assert.IsType<BadRequestResult>(result);
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
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void RemoveUserFromGroup_ReturnsOk_WhenUserOwnsGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userToRemove = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group> { new Group { Id = groupId } } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.removeUserFromGroup(new RemoveUserFromGroupRequestDto { SessionToken = userId, GroupGuid = groupId, UserToRemove = userToRemove });
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(x => x.RemoveUserFromGroup(userToRemove, groupId), Times.Once);
    }

    [Fact]
    public void RemoveUserFromGroup_ReturnsBadRequest_WhenUserIsNull()
    {
        var badToken = Guid.NewGuid();
        _dbMock.Setup(x => x.GetUserFromToken(badToken)).Returns((User)null);
        var result = _service.removeUserFromGroup(new RemoveUserFromGroupRequestDto { SessionToken = badToken, GroupGuid = Guid.NewGuid(), UserToRemove = Guid.NewGuid() });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void RemoveUserFromGroup_ReturnsBadRequest_WhenUserNotInGroup()
    {
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Groups = new List<Group>() };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        var result = _service.removeUserFromGroup(new RemoveUserFromGroupRequestDto { SessionToken = userId, GroupGuid = groupId, UserToRemove = Guid.NewGuid() });
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void IsGroupPrivate_ReturnsOk_WhenGroupExists()
    {
        var groupId = Guid.NewGuid();
        _dbMock.Setup(x => x.FindGroupById(groupId)).Returns(new Group { Id = groupId, IsPrivate = true });
        var result = _service.IsGroupPrivate(new IsGroupPrivateRequestDto { GroupId = groupId });
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<IsGroupPrivateResponseDto>(ok.Value);
        Assert.True(dto.IsPrivate);
    }

    [Fact]
    public void IsGroupPrivate_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        _dbMock.Setup(x => x.FindGroupById(It.IsAny<Guid>())).Returns((Group)null);
        var result = _service.IsGroupPrivate(new IsGroupPrivateRequestDto { GroupId = Guid.NewGuid() });
        Assert.IsType<NotFoundResult>(result);
    }
}
