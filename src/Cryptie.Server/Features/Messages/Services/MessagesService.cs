using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages.Services;

public class MessagesService(IDatabaseService databaseService, IMessageHubService messageHubService)
    : ControllerBase, IMessagesService
{
    public IActionResult SendMessage([FromBody] SendMessageRequestDto sendMessageRequest)
    {
        var user = databaseService.GetUserFromToken(sendMessageRequest.SenderToken);
        if (user == null) return Unauthorized();

        var group = databaseService.FindGroupById(sendMessageRequest.GroupId);
        if (group == null) return NotFound();

        if (!group.Members.Any(m => m.Id == user.Id)) return BadRequest();

        if (string.IsNullOrWhiteSpace(sendMessageRequest.Message)) return BadRequest("Message cannot be empty.");

        var message = databaseService.SendGroupMessage(group, user, sendMessageRequest.Message);
        messageHubService.SendMessageToGroup(group.Id, user.Id, message.Message);
        return Ok();
    }

    public IActionResult GetMessage([FromBody] GetMessageRequestDto getMessageRequest)
    {
        var user = databaseService.GetUserFromToken(getMessageRequest.UserToken);
        if (user == null) return Unauthorized();

        var group = databaseService.FindGroupById(getMessageRequest.GroupId);
        if (group == null) return NotFound();

        if (!group.Members.Any(m => m.Id == user.Id)) return BadRequest();

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

    public IActionResult GetGroupMessages([FromBody] GetGroupMessagesRequestDto request)
    {
        var user = databaseService.GetUserFromToken(request.UserToken);
        if (user == null) return Unauthorized();

        var group = databaseService.FindGroupById(request.GroupId);
        if (group == null) return NotFound();

        if (!group.Members.Any(m => m.Id == user.Id)) return BadRequest();

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

    public IActionResult GetGroupMessagesSince([FromBody] GetGroupMessagesSinceRequestDto request)
    {
        var user = databaseService.GetUserFromToken(request.UserToken);
        if (user == null) return Unauthorized();

        var group = databaseService.FindGroupById(request.GroupId);
        if (group == null) return NotFound();

        if (!group.Members.Any(m => m.Id == user.Id)) return BadRequest();

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