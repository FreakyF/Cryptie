using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            (userId, groupId) =>
                _joinedSubject.OnNext(new SignalRJoined(groupId, userId)));

        _hubConnection.On<Guid, string, Guid>(
            "ReceiveGroupMessage",
            (senderId, text, groupId) =>
                _messageSubject.OnNext(new SignalRMessage(groupId, text, senderId)));

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
                            /* retry */
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

        foreach (var gid in groupIds)
            await _hubConnection.InvokeAsync("JoinGroup", userId, gid);
    }

    public async Task<IList<GetGroupMessagesResponseDto.MessageDto>> GetGroupMessagesAsync(
        Guid userToken,
        Guid groupId)
    {
        var getAllDto = new GetGroupMessagesRequestDto
        {
            UserToken = userToken,
            GroupId = groupId
        };

        // Spróbuj najpierw GET /messages/get-all z body
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "/messages/get-all")
            {
                Content = JsonContent.Create(getAllDto)
            };
            var resp = await _httpClient.SendAsync(req);
            if (resp.IsSuccessStatusCode)
            {
                var wrapper = await resp.Content
                    .ReadFromJsonAsync<GetGroupMessagesResponseDto>();
                if (wrapper?.Messages != null)
                    return wrapper.Messages;
            }
        }
        catch (HttpRequestException)
        {
            /* serwer nie obsługuje GET z body → fallback */
        }

        // Fallback: POST /messages/get-all-since z Since = MinValue
        var sinceDto = new GetGroupMessagesSinceRequestDto
        {
            UserToken = userToken,
            GroupId = groupId,
            Since = DateTime.MinValue
        };

        var resp2 = await _httpClient.PostAsJsonAsync("/messages/get-all-since", sinceDto);
        if (!resp2.IsSuccessStatusCode)
            return Array.Empty<GetGroupMessagesResponseDto.MessageDto>();

        var wrapper2 = await resp2.Content
            .ReadFromJsonAsync<GetGroupMessagesSinceResponseDto>();

        return wrapper2?.Messages
                   .Select(m => new GetGroupMessagesResponseDto.MessageDto
                   {
                       MessageId = m.MessageId,
                       GroupId = m.GroupId,
                       SenderId = m.SenderId,
                       Message = m.Message,
                       DateTime = m.DateTime
                   })
                   .ToList()
               ?? [];
    }

    public async Task SendMessageToGroupViaHttpAsync(Guid senderToken, Guid groupId, string message)
    {
        var dto = new SendMessageRequestDto
        {
            SenderToken = senderToken,
            GroupId = groupId,
            Message = message
        };
        var resp = await _httpClient.PostAsJsonAsync("/messages/send", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task SendMessageToGroupAsync(Guid groupId, string message)
    {
        if (_hubConnection.State != HubConnectionState.Connected)
            throw new InvalidOperationException("Cannot send: hub not connected.");

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