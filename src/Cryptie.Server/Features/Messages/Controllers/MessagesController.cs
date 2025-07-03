using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Features.Messages.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages.Controllers;

[ApiController]
[Route("messages")]
public class MessagesController(IMessagesService messagesService)
    : ControllerBase
{
    [HttpPost("send", Name = "PostSendMessage")]
    public IActionResult SendMessage([FromBody] SendMessageRequestDto sendMessageRequest)
    {
        return messagesService.SendMessage(sendMessageRequest);
    }

    [HttpGet("get-all", Name = "GetGroupMessages")]
    public IActionResult GetGroupMessages([FromBody] GetGroupMessagesRequestDto groupMessagesRequest)
    {
        return messagesService.GetGroupMessages(groupMessagesRequest);
    }

    [HttpPost("get-all-since", Name = "GetGroupMessagesSince")]
    public IActionResult GetGroupMessagesSince([FromBody] GetGroupMessagesSinceRequestDto getGroupMessagesSinceRequest)
    {
        return messagesService.GetGroupMessagesSince(getGroupMessagesSinceRequest);
    }
}