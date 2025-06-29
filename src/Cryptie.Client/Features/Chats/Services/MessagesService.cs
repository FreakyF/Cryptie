using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Features.Chats.Services;

public class MessagesService : IMessagesService
{
    private readonly HttpClient _httpClient;
    private readonly HubConnection _hubConnection;
    private readonly Subject<SignalRMessage> _messageSubject = new();

    public MessagesService(HubConnection hubConnection, HttpClient httpClient)
    {
        _hubConnection = hubConnection
                         ?? throw new ArgumentNullException(nameof(hubConnection));

        _httpClient = httpClient
                      ?? throw new ArgumentNullException(nameof(httpClient));

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
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }

        foreach (var gid in groupIds)
            await _hubConnection.InvokeAsync("JoinGroup", userId, gid);
    }

    public async Task<IList<GetGroupMessagesResponseDto.MessageDto>> GetGroupMessagesAsync(
        Guid userToken, Guid groupId)
    {
        var dto = new GetGroupMessagesRequestDto { UserToken = userToken, GroupId = groupId };

        using var req = new HttpRequestMessage(HttpMethod.Get, "/messages/get-all");
        req.Content = JsonContent.Create(dto);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var resp = await _httpClient.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        var wrapper = await resp.Content.ReadFromJsonAsync<GetGroupMessagesResponseDto>();
        return wrapper?.Messages
               ?? [];
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