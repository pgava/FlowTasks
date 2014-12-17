using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:completeworkflow")]
    public class CompleteWorkflowRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public WorkflowStatusType WorkflowStatusType { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
