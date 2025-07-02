// using Cryptie.Common.Entities;
// using Cryptie.Common.Features.UserManagement.DTOs;
// using Cryptie.Server.Features.UserManagement;
// using Cryptie.Server.Features.UserManagement.Controllers;
// using Cryptie.Server.Services;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;
//
// namespace Cryptie.Server.Tests.Features.UserManagement;
//
// public class UserManagementControllerTests
// {
//     private readonly Mock<IDatabaseService> _dbMock;
//     private readonly UserManagementController _controller;
//
//     public UserManagementControllerTests()
//     {
//         _dbMock = new Mock<IDatabaseService>();
//         _controller = new UserManagementController(_dbMock.Object);
//     }
//
//     [Fact]
//     public void UserGuidFromToken_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { Id = Guid.NewGuid() };
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new UserGuidFromTokenRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserGuidFromToken(dto);
//         var ok = Assert.IsType<OkObjectResult>(result);
//         var response = Assert.IsType<UserGuidFromTokenResponseDto>(ok.Value);
//         Assert.Equal(user.Id, response.Guid);
//     }
//
//     [Fact]
//     public void UserGuidFromToken_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new UserGuidFromTokenRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserGuidFromToken(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void UserLoginFromToken_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { Login = "testlogin" };
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new UserLoginFromTokenRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserLoginFromToken(dto);
//         var ok = Assert.IsType<OkObjectResult>(result);
//         var response = Assert.IsType<UserLoginFromTokenResponseDto>(ok.Value);
//         Assert.Equal(user.Login, response.Login);
//     }
//
//     [Fact]
//     public void UserLoginFromToken_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new UserLoginFromTokenRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserLoginFromToken(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void AddFriend_ReturnsNotFound_WhenFriendNull()
//     {
//         _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns((User)null);
//         var dto = new AddFriendRequestDto { Friend = "friend", SessionToken = Guid.NewGuid(), EncryptionKeys = new Dictionary<Guid, string>() };
//         var result = _controller.AddFriend(dto);
//         Assert.IsType<NotFoundResult>(result);
//     }
//
//     [Fact]
//     public void AddFriend_ReturnsBadRequest_WhenUserNull()
//     {
//         var friend = new User { Id = Guid.NewGuid() };
//         _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(friend);
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new AddFriendRequestDto { Friend = "friend", SessionToken = Guid.NewGuid(), EncryptionKeys = new Dictionary<Guid, string>() };
//         var result = _controller.AddFriend(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void AddFriend_ReturnsBadRequest_WhenEncryptionKeysInvalid()
//     {
//         var user = new User { Id = Guid.NewGuid() };
//         var friend = new User { Id = Guid.NewGuid() };
//         _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(friend);
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new AddFriendRequestDto {
//             Friend = "friend",
//             SessionToken = Guid.NewGuid(),
//             EncryptionKeys = new Dictionary<Guid, string> { { Guid.NewGuid(), "key" } } // niepoprawny klucz
//         };
//         var result = _controller.AddFriend(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void AddFriend_ReturnsOk_WhenSuccess()
//     {
//         var user = new User { Id = Guid.NewGuid(), DisplayName = "User" };
//         var friend = new User { Id = Guid.NewGuid(), DisplayName = "Friend" };
//         _dbMock.Setup(x => x.FindUserByLogin(It.IsAny<string>())).Returns(friend);
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         _dbMock.Setup(x => x.CreateGroup(It.IsAny<string>(), true)).Returns(new Group { Id = Guid.NewGuid() });
//         var dto = new AddFriendRequestDto {
//             Friend = "friend",
//             SessionToken = Guid.NewGuid(),
//             EncryptionKeys = new Dictionary<Guid, string> { { user.Id, "key1" }, { friend.Id, "key2" } }
//         };
//         var result = _controller.AddFriend(dto);
//         Assert.IsType<OkResult>(result);
//     }
//
//     [Fact]
//     public void FriendList_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { Id = Guid.NewGuid(), Friends = new List<User> { new User { Id = Guid.NewGuid() }, new User { Id = Guid.NewGuid() } } };
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new FriendListRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.FriendList(dto);
//         var ok = Assert.IsType<OkObjectResult>(result);
//         var response = Assert.IsType<GetFriendListResponseDto>(ok.Value);
//         Assert.Equal(user.Friends.Select(f => f.Id), response.Friends);
//     }
//
//     [Fact]
//     public void FriendList_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new FriendListRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.FriendList(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void NameFromGuid_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { DisplayName = "TestName" };
//         _dbMock.Setup(x => x.FindUserById(It.IsAny<Guid>())).Returns(user);
//         var dto = new NameFromGuidRequestDto { Id = Guid.NewGuid() };
//         var result = _controller.NameFromGuid(dto);
//         var ok = Assert.IsType<OkObjectResult>(result);
//         var response = Assert.IsType<NameFromGuidResponseDto>(ok.Value);
//         Assert.Equal(user.DisplayName, response.Name);
//     }
//
//     [Fact]
//     public void NameFromGuid_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.FindUserById(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new NameFromGuidRequestDto { Id = Guid.NewGuid() };
//         var result = _controller.NameFromGuid(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void UserGroups_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { Id = Guid.NewGuid(), Groups = new List<Group> { new Group { Id = Guid.NewGuid() }, new Group { Id = Guid.NewGuid() } } };
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new UserGroupsRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserGroups(dto);
//         var ok = Assert.IsType<OkObjectResult>(result);
//         var response = Assert.IsType<UserGroupsResponseDto>(ok.Value);
//         Assert.Equal(user.Groups.Select(g => g.Id), response.Groups);
//     }
//
//     [Fact]
//     public void UserGroups_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new UserGroupsRequestDto { SessionToken = Guid.NewGuid() };
//         var result = _controller.UserGroups(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
//
//     [Fact]
//     public void UserDisplayName_ReturnsOk_WhenUserExists()
//     {
//         var user = new User { Id = Guid.NewGuid() };
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns(user);
//         var dto = new UserDisplayNameRequestDto { Token = Guid.NewGuid(), Name = "NoweImie" };
//         var result = _controller.UserDisplayName(dto);
//         Assert.IsType<OkResult>(result);
//         _dbMock.Verify(x => x.ChangeUserDisplayName(user, dto.Name), Times.Once);
//     }
//
//     [Fact]
//     public void UserDisplayName_ReturnsBadRequest_WhenUserNull()
//     {
//         _dbMock.Setup(x => x.GetUserFromToken(It.IsAny<Guid>())).Returns((User)null);
//         var dto = new UserDisplayNameRequestDto { Token = Guid.NewGuid(), Name = "NoweImie" };
//         var result = _controller.UserDisplayName(dto);
//         Assert.IsType<BadRequestResult>(result);
//     }
// }
//
