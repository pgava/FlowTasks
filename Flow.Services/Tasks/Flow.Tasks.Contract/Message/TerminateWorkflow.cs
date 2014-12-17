using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:terminateworkflow")]
    public class TerminateWorkflowRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }
   
        [DataMember]
        public string Message { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:terminateworkflow")]
    public class TerminateWorkflowResponse
    {
        [DataMember]
        public string Ack { get; set; }
    }

}
