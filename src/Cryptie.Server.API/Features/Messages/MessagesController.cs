using System.Net.WebSockets;
using Cryptie.Server.Domain.Features.Authentication.DTOs.MessagesController;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cryptie.Server.API.Features.Messages;

[ApiController]
[Route("message")]
public class MessagesController : ControllerBase
{
    [HttpGet("ws", Name = "WebSocketMessage")]
    public async Task WebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}