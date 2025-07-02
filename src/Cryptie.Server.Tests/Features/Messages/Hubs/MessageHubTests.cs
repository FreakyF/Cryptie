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
        Assert.Single(_clientProxyMock.Invocations);
        var invocation = _clientProxyMock.Invocations[0];
        Assert.Equal("SendCoreAsync", invocation.Method.Name);
        Assert.Equal("ReceiveMessage", invocation.Arguments[0]);
        var msgArgs = Assert.IsType<object[]>(invocation.Arguments[1]);
        Assert.Equal(message, msgArgs[0]);
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
        Assert.Single(_clientProxyMock.Invocations);
        var invocation = _clientProxyMock.Invocations[0];
        Assert.Equal("SendCoreAsync", invocation.Method.Name);
        Assert.Equal("UserJoinedGroup", invocation.Arguments[0]);
        var args = Assert.IsType<object[]>(invocation.Arguments[1]);
        Assert.Equal(new object[] { userId, groupId }, args);
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
        Assert.Single(_clientProxyMock.Invocations);
        var invocation = _clientProxyMock.Invocations[0];
        Assert.Equal("SendCoreAsync", invocation.Method.Name);
        Assert.Equal("ReceiveGroupMessage", invocation.Arguments[0]);
        var groupArgs = Assert.IsType<object[]>(invocation.Arguments[1]);
        Assert.Equal(senderId, groupArgs[0]);
        Assert.Equal(message, groupArgs[1]);
        Assert.Equal(groupId, groupArgs[2]);
    }
}
