using Cryptie.Common.Features.Messages.DTOs;
using Cryptie.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages.Services;

public class MessagesService(IDatabaseService databaseService, IMessageHubService messageHubService)
    : ControllerBase, IMessagesService
{
    /// <summary>
    ///     Sends a message to a specified group on behalf of the authenticated user.
    /// </summary>
    /// <param name="sendMessageRequest">Request containing message and authentication data.</param>
    /// <returns><see cref="OkResult"/> when the message is sent.</returns>
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

    /// <summary>
    ///     Retrieves all messages for a particular group.
    /// </summary>
    /// <param name="request">Request containing user token and group identifier.</param>
    /// <returns>List of messages.</returns>
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

    /// <summary>
    ///     Retrieves messages for a group that were sent after the specified timestamp.
    /// </summary>
    /// <param name="request">Request containing group id, token and timestamp.</param>
    /// <returns>List of messages sent since the provided date.</returns>
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