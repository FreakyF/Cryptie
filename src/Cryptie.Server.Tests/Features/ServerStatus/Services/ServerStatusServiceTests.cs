using Cryptie.Server.Features.ServerStatus.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cryptie.Server.Tests.Features.ServerStatus.Services;

public class ServerStatusServiceTests
{
    [Fact]
    public void GetServerStatus_ReturnsOkResult()
    {
        // Arrange
        var service = new ServerStatusService();

        // Act
        var result = service.GetServerStatus();

        // Assert
        Assert.IsType<OkResult>(result);
    }
}

