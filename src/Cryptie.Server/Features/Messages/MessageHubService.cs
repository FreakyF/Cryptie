using Microsoft.AspNetCore.SignalR;

namespace Cryptie.Server.Features.Messages;

public class MessageHubService : IMessageHubService
{
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageHubService(IHubContext<MessageHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void SendMessageToGroup(Guid group, string message)
    {
        _hubContext.Clients.Group(group.ToString()).SendAsync("ReceiveGroupMessage", message, group);
    }
}