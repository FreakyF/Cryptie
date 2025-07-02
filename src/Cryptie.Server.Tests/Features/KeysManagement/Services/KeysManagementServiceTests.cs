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
    public void getGroupsKey_ReturnsGroupKeys()
    {
        var userId = Guid.NewGuid();
        var groupId1 = Guid.NewGuid();
        var groupId2 = Guid.NewGuid();
        var sessionToken = "token";
        var user = new User
        {
            Id = userId,
            Groups = new[] { new Group { Id = groupId1 }, new Group { Id = groupId2 } }
        };
        var req = new GetGroupsKeyRequestDto { SessionToken = sessionToken };
        _dbMock.Setup(x => x.GetUserFromToken(sessionToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(groupId1)).Returns(new Group { Id = groupId1 });
        _dbMock.Setup(x => x.FindGroupById(groupId2)).Returns(new Group { Id = groupId2 });
        _dbMock.Setup(x => x.getGroupEncryptionKey(userId, groupId1)).Returns("key1");
        _dbMock.Setup(x => x.getGroupEncryptionKey(userId, groupId2)).Returns("key2");

        var result = _service.getGroupsKey(req) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<GetGroupsKeyResponseDto>(result.Value);
        Assert.Equal(2, response.Keys.Count);
        Assert.Equal("key1", response.Keys[groupId1]);
        Assert.Equal("key2", response.Keys[groupId2]);
    }

    [Fact]
    public void getGroupsKey_ReturnsUnauthorized_WhenUserNotFound()
    {
        var req = new GetGroupsKeyRequestDto { SessionToken = "invalid" };
        _dbMock.Setup(x => x.GetUserFromToken("invalid")).Returns((User)null);

        var result = _service.getGroupsKey(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void getGroupsKey_ReturnsEmpty_WhenUserHasNoGroups()
    {
        var userId = Guid.NewGuid();
        var sessionToken = "token";
        var user = new User { Id = userId, Groups = new Group[0] };
        var req = new GetGroupsKeyRequestDto { SessionToken = sessionToken };
        _dbMock.Setup(x => x.GetUserFromToken(sessionToken)).Returns(user);

        var result = _service.getGroupsKey(req) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<GetGroupsKeyResponseDto>(result.Value);
        Assert.Empty(response.Keys);
    }

}
