using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TaskProperty
    {
        public int TaskPropertyId { get; set; }
        [MaxLength(20)]
        [Required]
        public string TaskCode { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }

        public int WorkflowCodeId { get; set; }
        public WorkflowCode WorkflowCode { get; set; }
    }
}
