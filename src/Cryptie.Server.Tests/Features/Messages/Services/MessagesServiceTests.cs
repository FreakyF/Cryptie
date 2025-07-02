using Cryptie.Common.Entities;
using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Features.Messages.Services;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cryptie.Server.Tests.Features.Messages.Services;

public class MessagesServiceTests
{
    private readonly Mock<IDatabaseService> _dbMock = new();
    private readonly Mock<IMessageHubService> _hubMock = new();
    private readonly MessagesService _service;

    public MessagesServiceTests()
    {
        _service = new MessagesService(_dbMock.Object, _hubMock.Object);
    }

    // SendMessage tests
    [Fact]
    public void SendMessage_Unauthorized()
    {
        var req = new SendMessageRequestDto { SenderToken = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.SenderToken)).Returns((User)null!);
        var result = _service.SendMessage(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void SendMessage_GroupNotFound()
    {
        var req = new SendMessageRequestDto { SenderToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.SenderToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns((Group)null!);
        var result = _service.SendMessage(req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void SendMessage_UserNotMember()
    {
        var req = new SendMessageRequestDto { SenderToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User>() };
        _dbMock.Setup(x => x.GetUserFromToken(req.SenderToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        var result = _service.SendMessage(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void SendMessage_EmptyMessage()
    {
        var req = new SendMessageRequestDto { SenderToken = Guid.NewGuid(), GroupId = Guid.NewGuid(), Message = "   " };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User> { user } };
        _dbMock.Setup(x => x.GetUserFromToken(req.SenderToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        var result = _service.SendMessage(req);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void SendMessage_Success()
    {
        var req = new SendMessageRequestDto { SenderToken = Guid.NewGuid(), GroupId = Guid.NewGuid(), Message = "msg" };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User> { user } };
        var msg = new GroupMessage { Id = Guid.NewGuid(), GroupId = group.Id, SenderId = user.Id, Message = req.Message, DateTime = DateTime.UtcNow };
        _dbMock.Setup(x => x.GetUserFromToken(req.SenderToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        _dbMock.Setup(x => x.SendGroupMessage(group, user, req.Message)).Returns(msg);
        _hubMock.Setup(x => x.SendMessageToGroup(group.Id, user.Id, msg.Message));
        var result = _service.SendMessage(req);
        Assert.IsType<OkResult>(result);
    }

    // GetMessage tests
    [Fact]
    public void GetMessage_Unauthorized()
    {
        var req = new GetMessageRequestDto { UserToken = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns((User)null!);
        var result = _service.GetMessage(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetMessage_GroupNotFound()
    {
        var req = new GetMessageRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns((Group)null!);
        var result = _service.GetMessage(req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetMessage_UserNotMember()
    {
        var req = new GetMessageRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User>() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        var result = _service.GetMessage(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void GetMessage_Success()
    {
        var req = new GetMessageRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid(), MessageId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User> { user } };
        var msg = new GroupMessage { Id = req.MessageId, GroupId = group.Id, SenderId = user.Id, Message = "msg", DateTime = DateTime.UtcNow };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        _dbMock.Setup(x => x.GetGroupMessage(req.MessageId, group.Id)).Returns(msg);
        var result = _service.GetMessage(req) as OkObjectResult;
        Assert.NotNull(result);
    }

    // GetGroupMessages tests
    [Fact]
    public void GetGroupMessages_Unauthorized()
    {
        var req = new GetGroupMessagesRequestDto { UserToken = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns((User)null!);
        var result = _service.GetGroupMessages(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetGroupMessages_GroupNotFound()
    {
        var req = new GetGroupMessagesRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns((Group)null!);
        var result = _service.GetGroupMessages(req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetGroupMessages_UserNotMember()
    {
        var req = new GetGroupMessagesRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User>() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        var result = _service.GetGroupMessages(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void GetGroupMessages_Success()
    {
        var req = new GetGroupMessagesRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User> { user } };
        var msg = new GroupMessage { Id = Guid.NewGuid(), GroupId = group.Id, SenderId = user.Id, Message = "msg", DateTime = DateTime.UtcNow };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        _dbMock.Setup(x => x.GetGroupMessages(req.GroupId)).Returns(new List<GroupMessage> { msg });
        var result = _service.GetGroupMessages(req) as OkObjectResult;
        Assert.NotNull(result);
    }

    // GetGroupMessagesSince tests
    [Fact]
    public void GetGroupMessagesSince_Unauthorized()
    {
        var req = new GetGroupMessagesSinceRequestDto { UserToken = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns((User)null!);
        var result = _service.GetGroupMessagesSince(req);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetGroupMessagesSince_GroupNotFound()
    {
        var req = new GetGroupMessagesSinceRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns((Group)null!);
        var result = _service.GetGroupMessagesSince(req);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetGroupMessagesSince_UserNotMember()
    {
        var req = new GetGroupMessagesSinceRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid() };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User>() };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        var result = _service.GetGroupMessagesSince(req);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void GetGroupMessagesSince_Success()
    {
        var req = new GetGroupMessagesSinceRequestDto { UserToken = Guid.NewGuid(), GroupId = Guid.NewGuid(), Since = DateTime.UtcNow.AddDays(-1) };
        var user = new User { Id = Guid.NewGuid() };
        var group = new Group { Id = req.GroupId, Members = new List<User> { user } };
        var msg = new GroupMessage { Id = Guid.NewGuid(), GroupId = group.Id, SenderId = user.Id, Message = "msg", DateTime = DateTime.UtcNow };
        _dbMock.Setup(x => x.GetUserFromToken(req.UserToken)).Returns(user);
        _dbMock.Setup(x => x.FindGroupById(req.GroupId)).Returns(group);
        _dbMock.Setup(x => x.GetGroupMessagesSince(req.GroupId, req.Since)).Returns(new List<GroupMessage> { msg });
        var result = _service.GetGroupMessagesSince(req) as OkObjectResult;
        Assert.NotNull(result);
    }
}