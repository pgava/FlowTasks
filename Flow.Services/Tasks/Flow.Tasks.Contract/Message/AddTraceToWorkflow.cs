using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:addtracetoworkflow")]
    public class AddTraceToWorkflowRequest
    {
        [DataMember]
        public string WorkflowOid { get; set; }

        [DataMember]
        public string TaskOid { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
