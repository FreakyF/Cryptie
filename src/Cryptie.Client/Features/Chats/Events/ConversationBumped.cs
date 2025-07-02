using System;

namespace Cryptie.Client.Features.Chats.Events;

public sealed class ConversationBumped(Guid groupId, DateTime timestamp)
{
    public Guid GroupId { get; } = groupId;

    // ReSharper disable once UnusedMember.Global
    public DateTime Timestamp { get; } = timestamp;
}