namespace Cryptie.Common.Features.Messages.DTOs
{
    public class GetMessageRequestDto
    {
        public Guid UserToken { get; set; }
        public Guid GroupId { get; set; }
        public Guid MessageId { get; set; }
    }
}

