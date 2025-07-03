using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement.Controllers;
using Cryptie.Server.Features.GroupManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.GroupManagement;

public class GroupManagementControllerTests
{
    private readonly Mock<IGroupManagementService> _serviceMock;
    private readonly GroupManagementController _controller;

    public GroupManagementControllerTests()
    {
        _serviceMock = new Mock<IGroupManagementService>();
        _controller = new GroupManagementController(_serviceMock.Object);
    }

    [Fact]
    public void IsGroupsPrivate_CallsServiceAndReturnsResult()
    {
        var dto = new IsGroupsPrivateRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.IsGroupsPrivate(dto)).Returns(expected);
        var result = _controller.IsGroupsPrivate(dto);
        Assert.Equal(expected, result);
        _serviceMock.Verify(s => s.IsGroupsPrivate(dto), Times.Once);
    }

    [Fact]
    public void GetGroupsNames_CallsServiceAndReturnsResult()
    {
        var dto = new GetGroupsNamesRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.GetGroupsNames(dto)).Returns(expected);
        var result = _controller.IsGroupsPrivate(dto);
        Assert.Equal(expected, result);
        _serviceMock.Verify(s => s.GetGroupsNames(dto), Times.Once);
    }
}

