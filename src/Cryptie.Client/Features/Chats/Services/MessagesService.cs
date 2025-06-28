using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Features.Chats.Services;

public class MessagesService : IMessagesService
{
    private readonly HubConnection _hubConnection;
    private readonly Subject<SignalRMessage> _messageSubject = new();

    public MessagesService(HubConnection hubConnection)
    {
        _hubConnection = hubConnection
                         ?? throw new ArgumentNullException(nameof(hubConnection));

        _hubConnection.On<Guid, Guid>("UserJoinedGroup", (uid, gid) =>
            GroupJoined.Enqueue(new SignalRJoined(gid, uid)));

        _hubConnection.On<string, Guid>("ReceiveGroupMessage", (msg, gid) =>
        {
            var signal = new SignalRMessage(gid, msg);
            GroupMessages.Enqueue(signal);
            _messageSubject.OnNext(signal);
        });
    }

    public ConcurrentQueue<SignalRJoined> GroupJoined { get; } = new();
    public ConcurrentQueue<SignalRMessage> GroupMessages { get; } = new();

    public IObservable<SignalRMessage> MessageReceived => _messageSubject;

    public async Task ConnectAsync(Guid userId, IEnumerable<Guid> groupIds)
    {
        await _hubConnection.StartAsync();

        foreach (var gid in groupIds)
            await _hubConnection.InvokeAsync("JoinGroup", userId, gid);
    }

    public async Task SendMessageToGroupAsync(Guid groupId, string message)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
            throw new InvalidOperationException("SignalR hub is not connected.");

        await _hubConnection.InvokeAsync("SendMessageToGroup", groupId, message);
    }

    public async ValueTask DisposeAsync()
    {
        _messageSubject.Dispose();

        if (_hubConnection.State == HubConnectionState.Connected)
            await _hubConnection.StopAsync();

        await _hubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}