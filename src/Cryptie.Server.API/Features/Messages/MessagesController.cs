using Cryptie.Server.Domain.Features.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.API.Features.Messages;

[ApiController]
[Route("message")]
public class MessagesController(IMessageService messageService) : ControllerBase
{
    [HttpGet("ws", Name = "WebSocketMessage")]
    public async Task WebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await messageService.HandleAuthentication(webSocket, CancellationToken.None);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}