using System;
using System.Collections.Generic;

namespace Cryptie.Common.Features.Messages.DTOs
{
    public class GetGroupMessagesSinceResponseDto
    {
        public List<MessageDto> Messages { get; set; } = [];

        public class MessageDto
        {
            public Guid MessageId { get; set; }
            public Guid GroupId { get; set; }
            public Guid SenderId { get; set; }
            public string Message { get; set; } = string.Empty;
            public DateTime DateTime { get; set; }
        }
    }
}