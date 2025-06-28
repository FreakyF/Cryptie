using System;

namespace Cryptie.Client.Features.Chats.Entities;

public record SignalRMessage(Guid GroupId, string Message);