using Cryptie.Common.Entities;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Features.UserManagement;
using Cryptie.Server.Features.UserManagement.Controllers;
using Cryptie.Server.Features.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.UserManagement;

public class UserManagementControllerTests
{
    private readonly Mock<IUserManagementService> _serviceMock;
    private readonly UserManagementController _controller;

    public UserManagementControllerTests()
    {
        _serviceMock = new Mock<IUserManagementService>();
        _controller = new UserManagementController(_serviceMock.Object);
    }

    [Fact]
    public void UserGuidFromToken_CallsServiceAndReturnsResult()
    {
        var dto = new UserGuidFromTokenRequestDto();
        var expected = new OkObjectResult("test");
        _serviceMock.Setup(s => s.UserGuidFromToken(dto)).Returns(expected);
        var result = _controller.UserGuidFromToken(dto);
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void AddFriend_CallsServiceAndReturnsResult()
    {
        var dto = new AddFriendRequestDto();
        var expected = new OkObjectResult("test");
        _serviceMock.Setup(s => s.AddFriend(dto)).Returns(expected);
        var result = _controller.AddFriend(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NameFromGuid_CallsServiceAndReturnsResult()
    {
        var dto = new NameFromGuidRequestDto();
        var expected = new OkObjectResult("test");
        _serviceMock.Setup(s => s.NameFromGuid(dto)).Returns(expected);
        var result = _controller.NameFromGuid(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UserGroups_CallsServiceAndReturnsResult()
    {
        var dto = new UserGroupsRequestDto();
        var expected = new OkObjectResult("test");
        _serviceMock.Setup(s => s.UserGroups(dto)).Returns(expected);
        var result = _controller.UserGroups(dto);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UserDisplayName_CallsServiceAndReturnsResult()
    {
        var dto = new UserDisplayNameRequestDto();
        var expected = new OkObjectResult("test");
        _serviceMock.Setup(s => s.UserDisplayName(dto)).Returns(expected);
        var result = _controller.UserDisplayName(dto);
        Assert.Equal(expected, result);
    }
}
