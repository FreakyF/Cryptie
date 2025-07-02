using Cryptie.Common.Entities;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement.Services;
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
}
