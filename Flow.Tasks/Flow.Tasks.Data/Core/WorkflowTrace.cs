using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{
    public class WorkflowTrace
    {
        public int WorkflowTraceId { get; set; }
        public DateTime When { get; set; }
        [MaxLength(16)]
        public string User { get; set; }
        [MaxLength(20)]
        public string Action { get; set; }
        [MaxLength(20)]
        public string Result { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }

        public int WorkflowDefinitionId { get; set; }
        public WorkflowDefinition WorkflowDefinition { get; set; }

        public int TraceEventId { get; set; }
        public TraceEvent TraceEvent { get; set; }
    }
}
