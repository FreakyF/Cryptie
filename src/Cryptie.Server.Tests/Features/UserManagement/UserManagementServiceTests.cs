using System;
using System.Collections.Generic;
using System.Linq;
using Cryptie.Common.Entities;
using Cryptie.Common.Features.UserManagement.DTOs;
using Cryptie.Server.Features.UserManagement.Services;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.UserManagement;

public class UserManagementServiceTests
{
    private readonly Mock<IDatabaseService> _dbMock;
    private readonly UserManagementService _service;

    public UserManagementServiceTests()
    {
        _dbMock = new Mock<IDatabaseService>();
        _service = new UserManagementService(_dbMock.Object);
    }

    [Fact]
    public void UserGuidFromToken_ReturnsOk_WhenUserExists()
    {
        var user = new User { Id = Guid.NewGuid() };
        var token = Guid.NewGuid();
        var dto = new UserGuidFromTokenRequestDto { SessionToken = token };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns(user);
        var result = _service.UserGuidFromToken(dto) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<UserGuidFromTokenResponseDto>(result.Value);
        Assert.Equal(user.Id, response.Guid);
    }

    [Fact]
    public void UserGuidFromToken_ReturnsBadRequest_WhenUserNull()
    {
        var token = Guid.NewGuid();
        var dto = new UserGuidFromTokenRequestDto { SessionToken = token };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns((User)null);
        var result = _service.UserGuidFromToken(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void AddFriend_ReturnsNotFound_WhenFriendNull()
    {
        var token = Guid.NewGuid();
        var dto = new AddFriendRequestDto { Friend = "friend", SessionToken = token, EncryptionKeys = new Dictionary<Guid, string>() };
        _dbMock.Setup(d => d.FindUserByLogin("friend")).Returns((User)null);
        var result = _service.AddFriend(dto);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void AddFriend_ReturnsBadRequest_WhenUserNull()
    {
        var friend = new User { Id = Guid.NewGuid(), DisplayName = "f" };
        var token = Guid.NewGuid();
        var dto = new AddFriendRequestDto { Friend = "friend", SessionToken = token, EncryptionKeys = new Dictionary<Guid, string>() };
        _dbMock.Setup(d => d.FindUserByLogin("friend")).Returns(friend);
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns((User)null);
        var result = _service.AddFriend(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void AddFriend_ReturnsBadRequest_WhenEncryptionKeysInvalid()
    {
        var user = new User { Id = Guid.NewGuid(), DisplayName = "u" };
        var friend = new User { Id = Guid.NewGuid(), DisplayName = "f" };
        var invalidKey = Guid.NewGuid();
        var token = Guid.NewGuid();
        var dto = new AddFriendRequestDto
        {
            Friend = "friend",
            SessionToken = token,
            EncryptionKeys = new Dictionary<Guid, string> { { invalidKey, "key" } }
        };
        _dbMock.Setup(d => d.FindUserByLogin("friend")).Returns(friend);
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns(user);
        var result = _service.AddFriend(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void AddFriend_ReturnsOk_WhenValid()
    {
        var user = new User { Id = Guid.NewGuid(), DisplayName = "u" };
        var friend = new User { Id = Guid.NewGuid(), DisplayName = "f" };
        var token = Guid.NewGuid();
        var keys = new Dictionary<Guid, string>
        {
            { user.Id, "key1" },
            { friend.Id, "key2" }
        };
        var dto = new AddFriendRequestDto
        {
            Friend = "friend",
            SessionToken = token,
            EncryptionKeys = keys
        };
        var group = new Group { Id = Guid.NewGuid() };
        _dbMock.Setup(d => d.FindUserByLogin("friend")).Returns(friend);
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns(user);
        _dbMock.Setup(d => d.CreateGroup(It.IsAny<string>(), true)).Returns(group);
        var result = _service.AddFriend(dto);
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(d => d.AddFriend(user, friend), Times.Once);
        _dbMock.Verify(d => d.AddUserToGroup(user.Id, group.Id), Times.Once);
        _dbMock.Verify(d => d.AddUserToGroup(friend.Id, group.Id), Times.Once);
        _dbMock.Verify(d => d.AddGroupEncryptionKey(user.Id, group.Id, "key1"), Times.Once);
        _dbMock.Verify(d => d.AddGroupEncryptionKey(friend.Id, group.Id, "key2"), Times.Once);
    }

    [Fact]
    public void NameFromGuid_ReturnsBadRequest_WhenUserNull()
    {
        var dto = new NameFromGuidRequestDto { Id = Guid.NewGuid() };
        _dbMock.Setup(d => d.FindUserById(dto.Id)).Returns((User)null);
        var result = _service.NameFromGuid(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void NameFromGuid_ReturnsOk_WhenUserExists()
    {
        var user = new User { DisplayName = "name" };
        var dto = new NameFromGuidRequestDto { Id = Guid.NewGuid() };
        _dbMock.Setup(d => d.FindUserById(dto.Id)).Returns(user);
        var result = _service.NameFromGuid(dto) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<NameFromGuidResponseDto>(result.Value);
        Assert.Equal("name", response.Name);
    }

    [Fact]
    public void UserGroups_ReturnsBadRequest_WhenUserNull()
    {
        var token = Guid.NewGuid();
        var dto = new UserGroupsRequestDto { SessionToken = token };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns((User)null);
        var result = _service.UserGroups(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void UserGroups_ReturnsOk_WithGroups()
    {
        var group1 = new Group { Id = Guid.NewGuid() };
        var group2 = new Group { Id = Guid.NewGuid() };
        var token = Guid.NewGuid();
        var user = new User { Groups = new List<Group> { group1, group2 } };
        var dto = new UserGroupsRequestDto { SessionToken = token };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns(user);
        var result = _service.UserGroups(dto) as OkObjectResult;
        Assert.NotNull(result);
        var response = Assert.IsType<UserGroupsResponseDto>(result.Value);
        Assert.Contains(group1.Id, response.Groups);
        Assert.Contains(group2.Id, response.Groups);
    }

    [Fact]
    public void UserDisplayName_ReturnsBadRequest_WhenUserNull()
    {
        var token = Guid.NewGuid();
        var dto = new UserDisplayNameRequestDto { Token = token, Name = "newName" };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns((User)null);
        var result = _service.UserDisplayName(dto);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void UserDisplayName_ReturnsOk_WhenUserExists()
    {
        var user = new User();
        var token = Guid.NewGuid();
        var dto = new UserDisplayNameRequestDto { Token = token, Name = "newName" };
        _dbMock.Setup(d => d.GetUserFromToken(token)).Returns(user);
        var result = _service.UserDisplayName(dto);
        Assert.IsType<OkResult>(result);
        _dbMock.Verify(d => d.ChangeUserDisplayName(user, "newName"), Times.Once);
    }
}
