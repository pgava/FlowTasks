using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TopicUser
    {
        public int TopicUserId { get; set; }
        [MaxLength(16)]
        [Required]
        public string User { get; set; }
        
        public int TopicMessageId { get; set; }
        public TopicMessage TopicMessage { get; set; }

        public int TopicStatusId { get; set; }
        public TopicStatus TopicStatus { get; set; }
    }
}
