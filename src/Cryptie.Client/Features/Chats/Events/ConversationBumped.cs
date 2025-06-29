using System;

namespace Cryptie.Client.Features.Chats.Events;

public sealed class ConversationBumped(Guid groupId, DateTime timestamp)
{
    public Guid GroupId { get; } = groupId;
    public DateTime Timestamp { get; } = timestamp;
}