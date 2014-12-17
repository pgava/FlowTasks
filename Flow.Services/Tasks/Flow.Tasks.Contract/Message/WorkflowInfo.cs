using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class WorkflowInfo
    {
        public string WorkflowId 
        { 
            get
            {
                return WorkflowOid.ToString();
            }
        }

        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string ParentWorkflowId { get; set; }

        [DataMember]
        public Guid ParentWorkflowOid { get; set; }

        public string CompletedOn 
        { 
            get
            {
                return CompletedOnDate.HasValue ? CompletedOnDate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        [DataMember]
        public DateTime? CompletedOnDate { get; set; }

        public string StartedOn
        {
            get
            {
                return StartedOnDate.HasValue ? StartedOnDate.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }


        [DataMember]
        public DateTime? StartedOnDate { get; set; }

    }
}
