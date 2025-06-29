namespace Cryptie.Common.Features.Messages.DTOs
{
    public class SendMessageRequestDto
    {
        public string Message { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderToken { get; set; }
    }
}