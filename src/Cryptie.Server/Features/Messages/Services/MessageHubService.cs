using Cryptie.Server.Features.Messages.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Cryptie.Server.Features.Messages.Services;

public class MessageHubService : IMessageHubService
{
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageHubService(IHubContext<MessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>
    /// Sends a real-time message to all clients in the specified SignalR group.
    /// </summary>
    /// <param name="group">Group identifier.</param>
    /// <param name="senderId">User sending the message.</param>
    /// <param name="message">Encrypted message content.</param>
    public void SendMessageToGroup(Guid group, Guid senderId, string message)
    {
        _hubContext.Clients.Group(group.ToString())
            .SendAsync("ReceiveGroupMessage", senderId, message, group);
    }
}