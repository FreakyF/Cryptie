using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Features.KeysManagement.Controllers;
using Cryptie.Server.Features.KeysManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.KeysManagement.Controllers;

public class KeysManagementControllerTests
{
    private readonly Mock<IKeysManagementService> _serviceMock;
    private readonly KeysManagementController _controller;

    public KeysManagementControllerTests()
    {
        _serviceMock = new Mock<IKeysManagementService>();
        _controller = new KeysManagementController(_serviceMock.Object);
    }

    [Fact]
    public void getUserKey_CallsServiceAndReturnsResult()
    {
        // Arrange
        var request = new GetUserKeyRequestDto();
        var expectedResult = new OkResult();
        _serviceMock.Setup(s => s.getUserKey(request)).Returns(expectedResult);

        // Act
        var result = _controller.getUserKey(request);

        // Assert
        Assert.Equal(expectedResult, result);
        _serviceMock.Verify(s => s.getUserKey(request), Times.Once);
    }

    [Fact]
    public void getGroupsKey_CallsServiceAndReturnsResult()
    {
        // Arrange
        var request = new GetGroupsKeyRequestDto();
        var expectedResult = new OkResult();
        _serviceMock.Setup(s => s.getGroupsKey(request)).Returns(expectedResult);
        
        // Act
        var result = _controller.getGroupKey(request);

        // Assert
        Assert.Equal(expectedResult, result);
        _serviceMock.Verify(s => s.getGroupsKey(request), Times.Once);
    }
}

