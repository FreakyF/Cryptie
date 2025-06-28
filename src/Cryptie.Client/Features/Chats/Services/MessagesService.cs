using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace Cryptie.Client.Features.Chats.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly Subject<SignalRMessage> _messageSubject = new();
        private HubConnection _hubConnection;

        public ConcurrentQueue<SignalRJoined> GroupJoined { get; } = new();
        public ConcurrentQueue<SignalRMessage> GroupMessages { get; } = new();

        public IObservable<SignalRMessage> MessageReceived => _messageSubject;

        public async Task ConnectAsync(Guid userId, IEnumerable<Guid> groupIds)
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

            _hubConnection.On<Guid, Guid>("UserJoinedGroup", (joinedUserId, groupId) =>
                GroupJoined.Enqueue(new SignalRJoined(groupId, joinedUserId)));

            _hubConnection.On<string, Guid>("ReceiveGroupMessage", (message, groupId) =>
            {
                var signal = new SignalRMessage(groupId, message);
                GroupMessages.Enqueue(signal);
                _messageSubject.OnNext(signal);
            });

            await _hubConnection.StartAsync();
            foreach (var gid in groupIds)
                await _hubConnection.InvokeAsync("JoinGroup", userId, gid);
        }

        public async Task SendMessageToGroupAsync(Guid groupId, string message)
        {
            if (_hubConnection is null)
                throw new InvalidOperationException("Najpierw wywołaj ConnectAsync(...)");

            await _hubConnection.InvokeAsync("SendMessageToGroup", groupId, message);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }

            _messageSubject.Dispose();
        }
    }
}