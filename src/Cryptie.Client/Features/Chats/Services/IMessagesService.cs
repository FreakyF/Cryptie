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

    Task SendMessageToGroupViaHttpAsync(Guid senderToken, Guid groupId, string message);

    Task<GetMessageResponseDto> GetMessageFromGroupViaHttpAsync(
        Guid userToken, Guid groupId, Guid messageId);

    Task<IList<GetGroupMessagesSinceResponseDto.MessageDto>> GetGroupMessagesSinceAsync(
        Guid userToken, Guid groupId, DateTime since);
}