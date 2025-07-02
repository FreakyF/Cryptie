using Cryptie.Common.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement.Controllers;
using Cryptie.Server.Features.GroupManagement.Services;
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
}

