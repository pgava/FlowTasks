using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{

    /*
     1. The list come from TaskUsers all user included in the list can see the task.
     2. When user accept 
        - AcceptUser = ..., TaskUsers[i].InUse = true
     3. HandOver list come from TaskHandOverUsers. When user select one from the list
        - AcceptUser = null, TaskHandOverUsers.InUse = true, HandedOverStatus = HandedOver
     4. The list now come from TaskHandOverUsers where InUse = true, HandedOverStatus is HandedOver
     5. When user accept
        - AcceptUser = ...
     6. HandOver list come from TaskUsers with InUse = true. When user hand over
        - AcceptUser = null, TaskHandOverUsers.InUse = false, HandedOverStatus = HandedBack
     7. The list come from TaskUsers where InUse = true
        
     */

    public class TaskDefinition
    {
        public int TaskDefinitionId { get; set; }
        [Required]
        public Guid TaskOid { get; set; }
        public int TaskCorrelationId { get; set; }
        [MaxLength(50)]
        [Required]
        public string TaskCode { get; set; }
        [MaxLength(200)]
        [Required]
        public string UiCode { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(20)]
        public string DefaultResult { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? AcceptedOn { get; set; }
        [MaxLength(16)]
        public string AcceptedBy { get; set; }
        public DateTime? CompletedOn { get; set; }
        [MaxLength(16)]
        public string AcceptUser { get; set; }
        public bool CanBeHandedOver { get; set; }
        [MaxLength(20)]
        public string HandedOverStatus { get; set; } //HandedBack, HandedOver        

        public int WorkflowDefinitionId { get; set; }
        public WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
