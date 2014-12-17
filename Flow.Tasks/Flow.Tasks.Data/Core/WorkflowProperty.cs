using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class WorkflowProperty
    {
        public int WorkflowPropertyId { get; set; }
        [MaxLength(50)]
        public string Domain { get; set; }

        public int WorkflowCodeId { get; set; }
        public WorkflowCode WorkflowCode { get; set; }

        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
