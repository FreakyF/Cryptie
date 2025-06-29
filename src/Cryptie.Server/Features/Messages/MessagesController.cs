using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages;

[Route("messages")]
public class MessagesController : ControllerBase
{
    private readonly IDatabaseService databaseService;
    private readonly IMessageHubService messageHubService;

    public MessagesController(IDatabaseService databaseService, IMessageHubService messageHubService)
    {
        this.databaseService = databaseService;
        this.messageHubService = messageHubService;
    }

    [HttpPost("send", Name = "PostSendMessage")]
    public IActionResult SendMessage([FromBody] SendMessageRequestDto sendMessageRequest)
    {
        var user = databaseService.GetUserFromToken(sendMessageRequest.SenderToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var group = databaseService.FindGroupById(sendMessageRequest.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        if (!group.Members.Any(m => m.Id == user.Id))
        {
            return BadRequest("User is not a member of the group.");
        }

        if (string.IsNullOrWhiteSpace(sendMessageRequest.Message))
        {
            return BadRequest("Message cannot be empty.");
        }

        var message = databaseService.SendGroupMessage(group, user, sendMessageRequest.Message);
        messageHubService.SendMessageToGroup(group.Id, message.Id.ToString());
        return Ok();
    }

    [HttpGet("get", Name = "GetMessage")]
    public IActionResult GetMessage([FromBody] GetMessageRequestDto getMessageRequest)
    {
        var user = databaseService.GetUserFromToken(getMessageRequest.UserToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var group = databaseService.FindGroupById(getMessageRequest.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        if (!group.Members.Any(m => m.Id == user.Id))
        {
            return BadRequest("User is not a member of the group.");
        }

        var message = databaseService.GetGroupMessage(getMessageRequest.MessageId, group.Id);

        return Ok(new GetMessageResponseDto
        {
            MessageId = message.Id,
            GroupId = message.GroupId,
            SenderId = message.SenderId,
            Message = message.Message,
            DateTime = message.DateTime
        });
    }
}