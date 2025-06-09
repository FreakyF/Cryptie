using System;
using System.Collections.Concurrent;
using System.Net.Http;
using Cryptie.Common.Entities.User;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Features.Messages.Services;

public class MessagesService
{
    private HubConnection _hubConnection;
    public ConcurrentQueue<SignalRJoined> groupJoined { get; } = new();
    public ConcurrentQueue<SignalRMessage> groupMessages { get; } = new();

    public void ConnectToHub(User user)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7161/messages", options =>
            {
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.StartAsync().GetAwaiter().GetResult();

        _hubConnection.On<Guid, Guid>("UserJoinedGroup", (uId, gId) =>
            groupJoined.Enqueue(new SignalRJoined(gId, uId)));

        _hubConnection.On<string, Guid>("ReceiveGroupMessage", (msg, gId) =>
            groupMessages.Enqueue(new SignalRMessage(gId, msg)));

        foreach (var group in user.Groups)
        {
            _hubConnection.InvokeAsync("JoinGroup", user.Id, group.Id)
                .GetAwaiter().GetResult();
        }
    }

    public void SendMessageToGroup(string message, Guid groupId)
    {
        if (_hubConnection == null)
            throw new InvalidOperationException("Najpierw wywolaj ConnectToHub().");

        _hubConnection.InvokeAsync("SendMessageToGroup", groupId, message)
            .GetAwaiter().GetResult();
    }
}

public class SignalRMessage(Guid id, string message)
{
    public Guid Id { get; } = id;
    public string Message { get; } = message;
}

public class SignalRJoined(Guid id, Guid user)
{
    public Guid Id { get; } = id;
    public Guid User { get; } = user;
}