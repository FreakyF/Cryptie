using Cryptie.Server.Features.ServerStatus;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cryptie.Server.Tests.Features.ServerStatus;

public class ServerStatusControllerTests
{
    [Fact]
    public void GetServerStatus_ReturnsOk()
    {
        // Arrange
        var controller = new ServerStatusController();
        // Act
        var result = controller.GetServerStatus();
        // Assert
        Assert.IsType<OkResult>(result);
    }
}

