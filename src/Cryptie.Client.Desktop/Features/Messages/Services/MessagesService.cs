using System;
using System.Collections.Concurrent;
using Cryptie.Server.Domain.Features.Authentication.Entities.User;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Desktop.Features.Messages.Services;

public class MessagesService
{
    public ConcurrentQueue<SignalRJoined> groupJoined = new ConcurrentQueue<SignalRJoined>();
    public ConcurrentQueue<SignalRMessage> groupMessages = new ConcurrentQueue<SignalRMessage>();
    private HubConnection hubConnection;

    public void ConnectToHub(User user)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7161/messages")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<Guid, Guid>("UserJoinedGroup",
            (user, groupId) => { groupJoined.Enqueue(new SignalRJoined(groupId, user)); });

        hubConnection.On<string, Guid>("ReceiveGroupMessage",
            (message, groupId) => { groupMessages.Enqueue(new SignalRMessage(groupId, message)); });

        foreach (var group in user.Groups)
        {
            hubConnection.InvokeAsync("JoinChat", user.Id, group.Id);
        }
    }

    public void SendMessageToGroup(string message, Guid group)
    {
        hubConnection.InvokeAsync("SendMessageToGroup", group, message);
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