using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Cryptie.Server.Features.Messages;

public class MessageHub : Hub, IMessageHub
{
    private static readonly ConcurrentDictionary<string, Guid> Users = new();

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task JoinGroup(Guid user, Guid group)
    {
        Users.TryAdd(Context.ConnectionId, user);
        await Groups.AddToGroupAsync(Context.ConnectionId, group.ToString());
        await Clients.Group(group.ToString()).SendAsync("UserJoinedGroup", user, group);
    }

    public async Task SendMessageToGroup(Guid group, Guid senderId, string message)
    {
        await Clients
            .Group(group.ToString())
            .SendAsync("ReceiveGroupMessage", senderId, message, group);
    }
}