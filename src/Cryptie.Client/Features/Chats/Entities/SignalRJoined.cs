using System;

namespace Cryptie.Client.Features.Chats.Entities;

// ReSharper disable NotAccessedPositionalProperty.Global
public record SignalRJoined(Guid GroupId, Guid UserId);