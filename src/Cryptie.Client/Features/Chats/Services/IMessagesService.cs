using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptie.Client.Features.Chats.Entities;
using Cryptie.Common.Features.Messages.DTOs;

namespace Cryptie.Client.Features.Chats.Services;

public interface IMessagesService : IAsyncDisposable
{
    IObservable<SignalRMessage> MessageReceived { get; }

    Task ConnectAsync(Guid userId, IEnumerable<Guid> groupIds);

    Task<IList<GetGroupMessagesResponseDto.MessageDto>> GetGroupMessagesAsync(
        Guid userToken,
        Guid groupId);

    Task SendMessageToGroupAsync(Guid groupId, string message);
    Task SendMessageToGroupViaHttpAsync(Guid senderToken, Guid groupId, string message);
}