using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages;

[ApiController]
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
        messageHubService.SendMessageToGroup(group.Id, user.Id, message.Message);
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

    [HttpGet("get-all", Name = "GetGroupMessages")]
    public IActionResult GetGroupMessages([FromBody] GetGroupMessagesRequestDto request)
    {
        var user = databaseService.GetUserFromToken(request.UserToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var group = databaseService.FindGroupById(request.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        if (!group.Members.Any(m => m.Id == user.Id))
        {
            return BadRequest("User is not a member of the group.");
        }

        var messages = databaseService.GetGroupMessages(request.GroupId);
        var response = new GetGroupMessagesResponseDto
        {
            Messages = messages.Select(m => new GetGroupMessagesResponseDto.MessageDto
            {
                MessageId = m.Id,
                GroupId = m.GroupId,
                SenderId = m.SenderId,
                Message = m.Message,
                DateTime = m.DateTime
            }).ToList()
        };
        return Ok(response);
    }

    [HttpPost("get-all-since", Name = "GetGroupMessagesSince")]
    public IActionResult GetGroupMessagesSince([FromBody] GetGroupMessagesSinceRequestDto request)
    {
        var user = databaseService.GetUserFromToken(request.UserToken);
        if (user == null)
        {
            return Unauthorized();
        }

        var group = databaseService.FindGroupById(request.GroupId);
        if (group == null)
        {
            return NotFound();
        }

        if (!group.Members.Any(m => m.Id == user.Id))
        {
            return BadRequest("User is not a member of the group.");
        }

        var messages = databaseService.GetGroupMessagesSince(request.GroupId, request.Since);
        var response = new GetGroupMessagesSinceResponseDto
        {
            Messages = messages.Select(m => new GetGroupMessagesSinceResponseDto.MessageDto
            {
                MessageId = m.Id,
                GroupId = m.GroupId,
                SenderId = m.SenderId,
                Message = m.Message,
                DateTime = m.DateTime
            }).ToList()
        };
        return Ok(response);
    }
}