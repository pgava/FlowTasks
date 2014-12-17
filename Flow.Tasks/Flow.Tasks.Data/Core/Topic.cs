using System;
using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class Topic
    {
        public int TopicId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        
        public DateTime LastChanged { get; set; }
    }
}
