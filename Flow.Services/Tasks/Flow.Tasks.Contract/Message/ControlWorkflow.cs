using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:controlworkflow")]
    public class ControlWorkflowRequest
    {
        [DataMember]
        public string WorkflowOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:controlworkflow")]
    public class ControlWorkflowResponse
    {
        [DataMember]
        public string Message { get; set; }
    }

}
