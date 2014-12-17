using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{
    public class WorkflowDefinition
    {
        public int WorkflowDefinitionId { get; set; }
        [Required]
        public Guid WorkflowOid { get; set; }
        public DateTime? CompletedOn { get; set; }
        public DateTime StartedOn { get; set; }
        [MaxLength(50)]
        public string Domain { get; set; }

        public int? WorkflowParentDefinitionId { get; set; }
        public WorkflowDefinition WorkflowParentDefinition { get; set; }

        public int WorkflowCodeId { get; set; }
        public WorkflowCode WorkflowCode { get; set; }

        public int WorkflowStatusId { get; set; }
        public WorkflowStatus WorkflowStatus { get; set; }

    }
}
