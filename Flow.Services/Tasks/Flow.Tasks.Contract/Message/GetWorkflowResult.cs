using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getworkflowresultrequest")]
    public class GetWorkflowResultRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getworkflowresultrequest")]
    public class GetWorkflowResultResponse
    {
        [DataMember]
        public WorkflowResultInfo WorkflowResultInfo { get; set; }
    }
}
