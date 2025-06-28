using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.GroupManagement;

public class GroupManagementControllerTest
{
    private readonly Mock<IGroupManagementService> _serviceMock;
    private readonly GroupManagementController _controller;

    public GroupManagementControllerTest()
    {
        _serviceMock = new Mock<IGroupManagementService>();
        _controller = new GroupManagementController(_serviceMock.Object);
    }

    [Fact]
    public void GetName_ReturnsServiceResult()
    {
        var dto = new GetGroupNameRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.getName(dto)).Returns(expected);
        var result = _controller.getName(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CreateGroup_ReturnsServiceResult()
    {
        var dto = new CreateGroupRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.createGroup(dto)).Returns(expected);
        var result = _controller.createGroup(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void DeleteGroup_ReturnsServiceResult()
    {
        var dto = new DeleteGroupRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.deleteGroup(dto)).Returns(expected);
        var result = _controller.deleteGroup(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ChangeGroupName_ReturnsServiceResult()
    {
        var dto = new ChangeGroupNameRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.changeGroupName(dto)).Returns(expected);
        var result = _controller.changeGroupName(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddUserToGroup_ReturnsServiceResult()
    {
        var dto = new AddUserToGroupRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.addUserToGroup(dto)).Returns(expected);
        var result = _controller.addUserToGroup(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RemoveUserFromGroup_ReturnsServiceResult()
    {
        var dto = new RemoveUserFromGroupRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.removeUserFromGroup(dto)).Returns(expected);
        var result = _controller.removeUserFromGroup(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsGroupPrivate_ReturnsServiceResult()
    {
        var dto = new IsGroupPrivateRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.IsGroupPrivate(dto)).Returns(expected);
        var result = _controller.IsGroupPrivate(dto);
        Assert.Equal(expected, result);
    }
}

