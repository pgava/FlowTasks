using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TraceEvent
    {
        public int TraceEventId { get; set; }
        [MaxLength(20)]
        [Required]
        public string Type { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
