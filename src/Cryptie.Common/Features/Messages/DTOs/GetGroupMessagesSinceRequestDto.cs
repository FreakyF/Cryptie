using System;

namespace Cryptie.Common.Features.Messages.DTOs
{
    public class GetGroupMessagesSinceRequestDto
    {
        public Guid UserToken { get; set; }
        public Guid GroupId { get; set; }
        public DateTime Since { get; set; }
    }
}

