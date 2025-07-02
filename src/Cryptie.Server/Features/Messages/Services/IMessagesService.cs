using Cryptie.Common.Features.Messages.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cryptie.Server.Features.Messages.Services;

public interface IMessagesService
{
    public IActionResult SendMessage([FromBody] SendMessageRequestDto sendMessageRequest);

    public IActionResult GetMessage([FromBody] GetMessageRequestDto getMessageRequest);

    public IActionResult GetGroupMessages([FromBody] GetGroupMessagesRequestDto request);

    public IActionResult GetGroupMessagesSince([FromBody] GetGroupMessagesSinceRequestDto request);
}