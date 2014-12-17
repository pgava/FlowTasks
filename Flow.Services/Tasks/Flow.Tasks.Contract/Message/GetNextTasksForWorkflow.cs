using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getnexttasksforworkflow")]
    public class GetNextTasksForWorkflowRequest
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getnexttasksforworkflow")]
    [KnownType(typeof(TaskInfo))]
    public class GetNextTasksForWorkflowResponse
    {
        [DataMember]
        public TaskInfo[] Tasks { get; set; }
    }
}
