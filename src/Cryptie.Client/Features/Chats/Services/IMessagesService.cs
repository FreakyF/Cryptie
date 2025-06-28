using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;

namespace Cryptie.Client.Features.Chats.Services
{
    public interface IMessagesService : IAsyncDisposable
    {
        ConcurrentQueue<SignalRJoined> GroupJoined { get; }
        ConcurrentQueue<SignalRMessage> GroupMessages { get; }
        IObservable<SignalRMessage> MessageReceived { get; }

        Task ConnectAsync(Guid userId, IEnumerable<Guid> groupIds);
        Task SendMessageToGroupAsync(Guid groupId, string message);
    }
}