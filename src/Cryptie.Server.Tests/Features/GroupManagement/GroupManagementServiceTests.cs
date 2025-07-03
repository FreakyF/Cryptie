using Cryptie.Common.Entities;
using Cryptie.Common.Features.GroupManagement;
using Cryptie.Common.Features.GroupManagement.DTOs;
using Cryptie.Server.Features.GroupManagement;
using Cryptie.Server.Features.GroupManagement.Services;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cryptie.Server.Tests.Features.GroupManagement;

public class GroupManagementServiceTests
{
    private readonly Mock<IDatabaseService> _dbMock;
    private readonly GroupManagementService _service;

    public GroupManagementServiceTests()
    {
        _dbMock = new Mock<IDatabaseService>();
        _service = new GroupManagementService(_dbMock.Object);
    }

    [Fact]
    public void GetGroupsNames_ReturnsUnauthorized_WhenUserIsNull()
    {
        _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
        var result = _service.GetGroupsNames(new GetGroupsNamesRequestDto { SessionToken = Guid.NewGuid() });
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetGroupsNames_ReturnsOk_WithPublicAndPrivateGroups()
    {
        var userId = Guid.NewGuid();
        var publicGroup = new Group { Id = Guid.NewGuid(), Name = "Public", IsPrivate = false };
        var privateGroup = new Group { Id = Guid.NewGuid(), Name = "Private", IsPrivate = true, Members = new List<User> { new User { Id = userId }, new User { Id = Guid.NewGuid(), DisplayName = "OtherUser" } } };
        var user = new User { Id = userId, Groups = new List<Group> { publicGroup, privateGroup } };
        _dbMock.Setup(x => x.GetUserFromToken(userId)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(publicGroup.Id)).Returns(publicGroup);
        _dbMock.Setup(x => x.FindGroupById(privateGroup.Id)).Returns(privateGroup);
        _dbMock.Setup(x => x.FindUserById(It.IsAny<Guid>())).Returns<Guid>(id => new User { Id = id, DisplayName = "OtherUser" });
        var result = _service.GetGroupsNames(new GetGroupsNamesRequestDto { SessionToken = userId });
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<GetGroupsNamesResponseDto>(ok.Value);
        Assert.Contains(publicGroup.Id, dto.GroupsNames.Keys);
        Assert.Contains(privateGroup.Id, dto.GroupsNames.Keys);
    }
}
