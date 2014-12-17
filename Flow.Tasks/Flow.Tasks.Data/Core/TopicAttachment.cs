using System;
using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TopicAttachment
    {
        public int TopicAttachmentId { get; set; }
        [MaxLength(200)]
        public string FileName { get; set; }
        public Guid OidDocument { get; set; }

        public int TopicMessageId { get; set; }
        public TopicMessage TopicMessage { get; set; }
    }
}
