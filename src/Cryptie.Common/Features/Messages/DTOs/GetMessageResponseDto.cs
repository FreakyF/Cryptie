namespace Cryptie.Common.Features.Messages.DTOs
{
    public class GetMessageResponseDto
    {
        public Guid MessageId { get; set; }
        public Guid GroupId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}