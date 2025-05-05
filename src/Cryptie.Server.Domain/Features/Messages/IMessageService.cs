using System.Net.WebSockets;

namespace Cryptie.Server.Domain.Features.Messages;

public interface IMessageService
{
    public Task HandleAuthentication(WebSocket webSocket, CancellationToken cancellationToken);
}