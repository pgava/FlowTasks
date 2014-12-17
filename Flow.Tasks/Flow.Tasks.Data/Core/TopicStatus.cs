using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TopicStatus
    {
        public int TopicStatusId { get; set; }
        [MaxLength(20)]
        [Required]
        public string Status { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
