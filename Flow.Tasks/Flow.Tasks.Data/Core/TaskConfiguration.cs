using System.ComponentModel.DataAnnotations;
namespace Flow.Tasks.Data.Core
{
    public class TaskConfiguration
    {
        public int TaskConfigurationId { get; set; }
        [MaxLength(20)]
        public string TaskCode { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public bool CanBeHandedOver { get; set; }
        [MaxLength(200)]
        public string HandOverUsers { get; set; }
        [MaxLength(200)]
        public string AssignedToUsers { get; set; }
        [MaxLength(20)]
        public string UiCode { get; set; }
        [MaxLength(20)]
        public string DefaultResult { get; set; }

        public int WorkflowCodeId { get; set; }
        public WorkflowCode WorkflowCode { get; set; }

    }
}
