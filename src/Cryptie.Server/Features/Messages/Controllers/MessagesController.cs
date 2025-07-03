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
/// <summary>
/// Sends an encrypted message to a group.
/// </summary>
/// <param name="sendMessageRequest">Message payload including group and text.</param>
/// <returns>HTTP result from the messaging service.</returns>
public IActionResult SendMessage([FromBody] SendMessageRequestDto sendMessageRequest)
{
    return messagesService.SendMessage(sendMessageRequest);
}

[HttpGet("get-all", Name = "GetGroupMessages")]
/// <summary>
/// Retrieves all messages for a specified group.
/// </summary>
/// <param name="groupMessagesRequest">Request identifying the group.</param>
/// <returns>Collection of messages.</returns>
public IActionResult GetGroupMessages([FromBody] GetGroupMessagesRequestDto groupMessagesRequest)
{
    return messagesService.GetGroupMessages(groupMessagesRequest);
}

[HttpPost("get-all-since", Name = "GetGroupMessagesSince")]
/// <summary>
/// Returns messages added to the group after a specific timestamp.
/// </summary>
/// <param name="getGroupMessagesSinceRequest">Request containing group identifier and timestamp.</param>
/// <returns>List of recent messages.</returns>
public IActionResult GetGroupMessagesSince([FromBody] GetGroupMessagesSinceRequestDto getGroupMessagesSinceRequest)
{
    return messagesService.GetGroupMessagesSince(getGroupMessagesSinceRequest);
}
}