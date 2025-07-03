using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Cryptie.Server.Features.Messages.Hubs;

public class MessageHub : Hub, IMessageHub
{
    private static readonly ConcurrentDictionary<string, Guid> Users = new();

    /// <summary>
    ///     Broadcasts a plain text message to all connected clients.
    /// </summary>
    /// <param name="message">Message to broadcast.</param>
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    /// <summary>
    ///     Adds the current connection to a SignalR group and notifies other members.
    /// </summary>
    /// <param name="user">Identifier of the joining user.</param>
    /// <param name="group">Group identifier.</param>
    public async Task JoinGroup(Guid user, Guid group)
    {
        Users.TryAdd(Context.ConnectionId, user);
        await Groups.AddToGroupAsync(Context.ConnectionId, group.ToString());
        await Clients.Group(group.ToString()).SendAsync("UserJoinedGroup", user, group);
    }

    /// <summary>
    ///     Sends a message to all members of a group.
    /// </summary>
    /// <param name="group">Group identifier.</param>
    /// <param name="senderId">Sender identifier.</param>
    /// <param name="message">Encrypted message payload.</param>
    public async Task SendMessageToGroup(Guid group, Guid senderId, string message)
    {
        await Clients
            .Group(group.ToString())
            .SendAsync("ReceiveGroupMessage", senderId, message, group);
    }
}