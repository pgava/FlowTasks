using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:restartworkflow")]
    [KnownType(typeof(WfProperty))]
    public class ReStartWorkflowRequest
    {
        [DataMember]
        public string OldWorkflowId { get; set; }

        [DataMember]
        public string RestartMode { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:restartworkflow")]
    public class ReStartWorkflowResponse
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public string Message { get; set; }

    }

}
