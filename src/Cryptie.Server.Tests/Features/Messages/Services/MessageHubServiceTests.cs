using System;
using System.Threading.Tasks;
using Cryptie.Server.Features.Messages.Hubs;
using Cryptie.Server.Features.Messages.Services;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace Cryptie.Server.Tests.Features.Messages.Services;

public class MessageHubServiceTests
{
    [Fact]
    public void SendMessageToGroup_CallsHubContextWithCorrectParameters()
    {
        // Arrange
        var hubContextMock = new Mock<IHubContext<MessageHub>>();
        var clientsMock = new Mock<IHubClients>();
        var groupClientMock = new Mock<IClientProxy>();
        var groupId = Guid.NewGuid();
        var senderId = Guid.NewGuid();
        var message = "test message";

        hubContextMock.Setup(x => x.Clients).Returns(clientsMock.Object);
        clientsMock.Setup(x => x.Group(groupId.ToString())).Returns(groupClientMock.Object);
        groupClientMock.Setup(x => x.SendCoreAsync(
            "ReceiveGroupMessage",
            It.Is<object[]>(args =>
                args.Length == 3 &&
                Equals(args[0], senderId) &&
                Equals(args[1], message) &&
                Equals(args[2], groupId)
            ),
            default
        )).Returns(Task.CompletedTask).Verifiable();

        var service = new MessageHubService(hubContextMock.Object);

        // Act
        service.SendMessageToGroup(groupId, senderId, message);

        // Assert
        groupClientMock.Verify(x => x.SendCoreAsync(
            "ReceiveGroupMessage",
            It.Is<object[]>(args =>
                args.Length == 3 &&
                Equals(args[0], senderId) &&
                Equals(args[1], message) &&
                Equals(args[2], groupId)
            ),
            default
        ), Times.Once);
    }
}
