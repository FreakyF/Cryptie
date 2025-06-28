using System;

namespace Cryptie.Client.Features.Chats.Services;

public record SignalRJoined(Guid GroupId, Guid UserId);