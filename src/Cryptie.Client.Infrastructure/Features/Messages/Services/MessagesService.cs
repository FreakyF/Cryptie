using System.Collections.Concurrent;
using Cryptie.Server.Domain.Features.Authentication.Entities.User;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Infrastructure.Features.Messages.Services;

public class MessagesService
{
    private HubConnection hubConnection;

    public ConcurrentQueue<SignalRJoined> groupJoined = new ConcurrentQueue<SignalRJoined>();
    public ConcurrentQueue<SignalRJoined> chatJoined = new ConcurrentQueue<SignalRJoined>();
    public ConcurrentQueue<SignalRMessage> groupMessages = new ConcurrentQueue<SignalRMessage>();
    public ConcurrentQueue<SignalRMessage> chatMessages = new ConcurrentQueue<SignalRMessage>();
    
    public void ConnectToHub(User user)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7161/messages")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<Guid, Guid>("UserJoinedGroup", (user, groupId) =>
        {
            groupJoined.Enqueue(new SignalRJoined(groupId, user));
        });
        
        hubConnection.On<Guid, Guid>("UserJoinedChat", (user, chatId) =>
        {
            chatJoined.Enqueue(new SignalRJoined(chatId, user));
        });
        
        hubConnection.On<string, Guid>("ReceiveGroupMessage", (message, groupId) =>
        {
            groupMessages.Enqueue(new SignalRMessage(groupId, message));
        });
        
        hubConnection.On<string, Guid>("ReceiveChatMessage", (message, chatId) =>
        {
            chatMessages.Enqueue(new SignalRMessage(chatId, message));
        });
        
        foreach (var group in user.Groups)
        {
            hubConnection.InvokeAsync("UserJoinedGroup", user.Id, group.Id);
        }
    }
}

public class SignalRMessage
{
    public Guid Id;
    public string Message;

    public SignalRMessage(Guid id, string message)
    {
        Id = id;
        Message = message;
    }
}

public class SignalRJoined
{
    public Guid Id;
    public Guid User;

    public SignalRJoined(Guid id, Guid user)
    {
        Id = id;
        User = user;
    }
}