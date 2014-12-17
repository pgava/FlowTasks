using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:isworkflowinprogressrequest")]
    public class IsWorkflowInProgressRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:isworkflowinprogressrequest")]
    public class IsWorkflowInProgressResponse
    {
        [DataMember]
        public bool IsInProgress { get; set; }
    }
}
