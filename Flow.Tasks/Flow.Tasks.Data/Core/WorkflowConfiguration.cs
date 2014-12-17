using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{
    public class WorkflowConfiguration
    {
        public int WorkflowConfigurationId { get; set; }        
        public string ServiceDefinition { get; set; }
        [MaxLength(256)]
        public string ServiceUrl { get; set; }
        [MaxLength(50)]
        public string BindingConfiguration { get; set; }
        [MaxLength(50)]
        public string ServiceEndpoint { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int WorkflowCodeId { get; set; }
        public WorkflowCode WorkflowCode { get; set; }

    }
}
