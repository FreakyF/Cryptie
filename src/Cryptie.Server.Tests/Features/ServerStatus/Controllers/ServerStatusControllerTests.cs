using Cryptie.Server.Features.ServerStatus.Controllers;
using Cryptie.Server.Features.ServerStatus.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.ServerStatus.Controllers;

public class ServerStatusControllerTests
{
    [Fact]
    public void GetServerStatus_CallsServiceAndReturnsResult()
    {
        // Arrange
        var serviceMock = new Mock<IServerStatusService>();
        var expectedResult = new OkResult();
        serviceMock.Setup(s => s.GetServerStatus()).Returns(expectedResult);
        var controller = new ServerStatusController(serviceMock.Object);

        // Act
        var result = controller.GetServerStatus();

        // Assert
        Assert.Same(expectedResult, result);
        serviceMock.Verify(s => s.GetServerStatus(), Times.Once);
    }
}

