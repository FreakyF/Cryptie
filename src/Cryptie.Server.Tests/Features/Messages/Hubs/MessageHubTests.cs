using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptie.Server.Features.Messages.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.Messages.Hubs;

public class MessageHubTests
{
    private readonly MessageHub _hub;
    private readonly Mock<IHubCallerClients> _clientsMock = new();
    private readonly Mock<IClientProxy> _clientProxyMock = new();
    private readonly Mock<IGroupManager> _groupsMock = new();
    private readonly Mock<HubCallerContext> _contextMock = new();

    public MessageHubTests()
    {
        _hub = new MessageHub
        {
            Clients = _clientsMock.Object,
            Groups = _groupsMock.Object,
            Context = _contextMock.Object
        };
    }

    [Fact]
    public async Task SendMessage_CallsClientsAllSendAsync()
    {
        _clientsMock.Setup(x => x.All).Returns(_clientProxyMock.Object);
        var message = "test message";
        await _hub.SendMessage(message);
        _clientsMock.Verify(x => x.All, Times.Once);
        _clientProxyMock.Verify(x => x.SendAsync("ReceiveMessage", message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task JoinGroup_AddsUserToGroupAndNotifiesGroup()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var connectionId = Guid.NewGuid().ToString();
        _contextMock.Setup(x => x.ConnectionId).Returns(connectionId);
        _groupsMock.Setup(x => x.AddToGroupAsync(connectionId, groupId.ToString(), default)).Returns(Task.CompletedTask);
        _clientsMock.Setup(x => x.Group(groupId.ToString())).Returns(_clientProxyMock.Object);

        await _hub.JoinGroup(userId, groupId);

        _groupsMock.Verify(x => x.AddToGroupAsync(connectionId, groupId.ToString(), default), Times.Once);
        _clientsMock.Verify(x => x.Group(groupId.ToString()), Times.Once);
        _clientProxyMock.Verify(x => x.SendAsync("UserJoinedGroup", userId, groupId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendMessageToGroup_CallsGroupSendAsync()
    {
        var groupId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var message = "group message";
        _clientsMock.Setup(x => x.Group(groupId.ToString())).Returns(_clientProxyMock.Object);

        await _hub.SendMessageToGroup(groupId, senderId, message);

        _clientsMock.Verify(x => x.Group(groupId.ToString()), Times.Once);
        _clientProxyMock.Verify(x => x.SendAsync("ReceiveGroupMessage", senderId, message, groupId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

