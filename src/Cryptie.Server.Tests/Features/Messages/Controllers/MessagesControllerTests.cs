using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Features.Messages.Controllers;
using Cryptie.Server.Features.Messages.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.Messages.Controllers;

public class MessagesControllerTests
{
    private readonly Mock<IMessagesService> _serviceMock = new();
    private readonly MessagesController _controller;

    public MessagesControllerTests()
    {
        _controller = new MessagesController(_serviceMock.Object);
    }

    [Fact]
    public void SendMessage_DelegatesToServiceAndReturnsResult()
    {
        var dto = new SendMessageRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.SendMessage(dto)).Returns(expected);
        var result = _controller.SendMessage(dto);
        Assert.Same(expected, result);
        _serviceMock.Verify(s => s.SendMessage(dto), Times.Once);
    }

    [Fact]
    public void GetMessage_DelegatesToServiceAndReturnsResult()
    {
        var dto = new GetMessageRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.GetMessage(dto)).Returns(expected);
        var result = _controller.GetMessage(dto);
        Assert.Same(expected, result);
        _serviceMock.Verify(s => s.GetMessage(dto), Times.Once);
    }

    [Fact]
    public void GetGroupMessages_DelegatesToServiceAndReturnsResult()
    {
        var dto = new GetGroupMessagesRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.GetGroupMessages(dto)).Returns(expected);
        var result = _controller.GetGroupMessages(dto);
        Assert.Same(expected, result);
        _serviceMock.Verify(s => s.GetGroupMessages(dto), Times.Once);
    }

    [Fact]
    public void GetGroupMessagesSince_DelegatesToServiceAndReturnsResult()
    {
        var dto = new GetGroupMessagesSinceRequestDto();
        var expected = new OkResult();
        _serviceMock.Setup(s => s.GetGroupMessagesSince(dto)).Returns(expected);
        var result = _controller.GetGroupMessagesSince(dto);
        Assert.Same(expected, result);
        _serviceMock.Verify(s => s.GetGroupMessagesSince(dto), Times.Once);
    }
}

