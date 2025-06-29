namespace Cryptie.Common.Features.Messages.DTOs
{
    public class GetGroupMessagesRequestDto
    {
        public Guid UserToken { get; set; }
        public Guid GroupId { get; set; }
    }
}

