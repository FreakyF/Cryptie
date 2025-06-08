using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Cryptie.Server.API.Features.Messages.Services;

public class MessageHub : Hub
{
    private static ConcurrentDictionary<string, Guid> _users = new();
    
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task JoinGroup(Guid user, Guid group)
    {
        _users.TryAdd(Context.ConnectionId, user);
        await Groups.AddToGroupAsync(Context.ConnectionId, group.ToString());
        await Clients.Group(group.ToString()).SendAsync("UserJoinedGroup", user, group);
    }

    public async Task JoinChat(Guid user, Guid chat)
    {
        _users.TryAdd(Context.ConnectionId, user);
        await Groups.AddToGroupAsync(Context.ConnectionId, chat.ToString());
        await Clients.Group(chat.ToString()).SendAsync("UserJoinedChat", user, chat);
    }

    public async Task SendMessageToGroup(Guid group, string message)
    {
        await Clients.Group(group.ToString()).SendAsync("ReceiveGroupMessage", message, group);
    }

    public async Task SendMessageToChat(Guid chat, string message)
    {
        await Clients.Group(chat.ToString()).SendAsync("ReceiveChatMessage", message, chat, chat);
    }
}