using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Features.Chats.Services;

public class MessagesService : IMessagesService
{
    private readonly HttpClient _httpClient;
    private readonly HubConnection _hubConnection;

    private readonly Subject<SignalRJoined> _joinedSubject = new();
    private readonly Subject<SignalRMessage> _messageSubject = new();

    private readonly SemaphoreSlim _startLock = new(1, 1);

    public MessagesService(HubConnection hubConnection, HttpClient httpClient)
    {
        _hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _hubConnection.On<Guid, Guid>("UserJoinedGroup",
            (userId, groupId) => { _joinedSubject.OnNext(new SignalRJoined(groupId, userId)); });

        _hubConnection.On<string, Guid>("ReceiveGroupMessage",
            (text, groupId) => { _messageSubject.OnNext(new SignalRMessage(groupId, text)); });

        _hubConnection.Closed += _ => Task.CompletedTask;
    }

    public IObservable<SignalRMessage> MessageReceived => _messageSubject;

    public async Task ConnectAsync(Guid userId, IEnumerable<Guid> groupIds)
    {
        await _startLock.WaitAsync();
        try
        {
            var isReady = false;

            while (!isReady)
            {
                switch (_hubConnection.State)
                {
                    case HubConnectionState.Disconnected:
                        try
                        {
                            await _hubConnection.StartAsync();
                            isReady = true;
                        }
                        catch (InvalidOperationException)
                        {
                            // Another thread raced us; loop around and re-check state
                        }

                        break;

                    case HubConnectionState.Connecting:
                    case HubConnectionState.Reconnecting:
                        await Task.Delay(50);
                        break;

                    case HubConnectionState.Connected:
                        isReady = true;
                        break;

                    default:
                        await Task.Delay(50);
                        break;
                }
            }
        }
        finally
        {
            _startLock.Release();
        }

        foreach (var groupId in groupIds)
        {
            await _hubConnection.InvokeAsync("JoinGroup", userId, groupId);
        }
    }

    public async Task<IList<GetGroupMessagesResponseDto.MessageDto>> GetGroupMessagesAsync(
        Guid userToken,
        Guid groupId)
    {
        var dto = new GetGroupMessagesRequestDto
        {
            UserToken = userToken,
            GroupId = groupId
        };

        using var request = new HttpRequestMessage(HttpMethod.Get, "/messages/get-all");
        request.Content = JsonContent.Create(dto);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var wrapper = await response.Content
            .ReadFromJsonAsync<GetGroupMessagesResponseDto>();

        return wrapper?.Messages ?? [];
    }

    public async Task SendMessageToGroupAsync(Guid groupId, string message)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
            throw new InvalidOperationException("Cannot send message: hub is not connected.");

        await _hubConnection.InvokeAsync("SendMessageToGroup", groupId, message);
    }

    public async ValueTask DisposeAsync()
    {
        _joinedSubject.Dispose();
        _messageSubject.Dispose();

        if (_hubConnection.State == HubConnectionState.Connected)
            await _hubConnection.StopAsync();

        await _hubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}