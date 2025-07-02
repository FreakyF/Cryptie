using Cryptie.Common.Entities;
using Cryptie.Server.Features.KeysManagement.Services;
using Cryptie.Common.Features.KeysManagement.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.KeysManagement.Services;

public class KeysManagementServiceTests
{
    private readonly Mock<IDatabaseService> _dbMock = new();
    private readonly KeysManagementService _service;

    public KeysManagementServiceTests()
    {
        _service = new KeysManagementService(_dbMock.Object);
    }

    [Fact]
    public void getUserKey_ReturnsPublicKey()
    {
        var userId = Guid.NewGuid();
        var expectedKey = "public-key";
        var req = new GetUserKeyRequestDto { UserId = userId };
        _dbMock.Setup(x => x.GetUserPublicKey(userId)).Returns(expectedKey);

        var result = _service.getUserKey(req) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<GetUserKeyResponseDto>(result.Value);
        Assert.Equal(expectedKey, response.PublicKey);
        _dbMock.Verify(x => x.GetUserPublicKey(userId), Times.Once);
    }

    [Fact]
    public void saveUserKeys_Unauthorized_WhenUserIsNull()
    {
        var req = new SaveUserKeysRequestDto { userToken = Guid.NewGuid(), privateKey = "priv", publicKey = "pub" };
        _dbMock.Setup(x => x.GetUserFromToken(req.userToken)).Returns((User?)null);

        var result = _service.saveUserKeys(req);
        Assert.IsType<UnauthorizedResult>(result);
        _dbMock.Verify(x => x.GetUserFromToken(req.userToken), Times.Once);
        _dbMock.Verify(x => x.SaveUserKeys(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void saveUserKeys_SavesKeysAndReturnsOk()
    {
        var req = new SaveUserKeysRequestDto { userToken = Guid.NewGuid(), privateKey = "priv", publicKey = "pub" };
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.userToken)).Returns(user);

        var result = _service.saveUserKeys(req);
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(x => x.GetUserFromToken(req.userToken), Times.Once);
        _dbMock.Verify(x => x.SaveUserKeys(user, req.privateKey, req.publicKey), Times.Once);
    }
}
