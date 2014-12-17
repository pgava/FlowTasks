using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TaskUserHandOver
    {
        public int TaskUserHandOverId { get; set; }
        [MaxLength(16)]
        [Required]
        public string User { get; set; }
        public bool InUse { get; set; }

        public int TaskDefinitionId { get; set; }
        public TaskDefinition TaskDefinition { get; set; }
    }
}
