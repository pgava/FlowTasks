using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TaskUser
    {
        public int TaskUserId { get; set; }
        [MaxLength(16)]
        [Required]
        public string User { get; set; }
        public bool InUse { get; set; }

        public int TaskDefinitionId { get; set; }
        public TaskDefinition TaskDefinition { get; set; }
    }
}
