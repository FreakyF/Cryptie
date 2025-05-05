using System.Net.WebSockets;
using System.Text.Json;
using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Domain.Features.Messages;

namespace Cryptie.Server.API.Features.Messages.Services;

public class MessagesService(IDatabaseService databaseService)
{
    public async Task HandleAuthentication(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024];
        using var ms = new MemoryStream();

        WebSocketReceiveResult result;
        do
        {
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            await ms.WriteAsync(buffer.AsMemory(0, result.Count), cancellationToken);
        } while (!result.EndOfMessage);

        ms.Seek(0, SeekOrigin.Begin);

        try
        {
            var authenticationRequestDto =
                await JsonSerializer.DeserializeAsync<AuthenticationRequestDTO>(ms,
                    cancellationToken: cancellationToken);
            var user = databaseService.FindUserByToken(authenticationRequestDto.Token);
        }
        catch
        {
            // ignored
        }
    }
}