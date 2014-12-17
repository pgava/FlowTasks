using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{
    public class TopicMessage
    {
        public int TopicMessageId { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        [MaxLength(20)]
        public string From { get; set; }
        [MaxLength(200)]
        public string To { get; set; }
        public DateTime When { get; set; }
        public Boolean IsTopic { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
