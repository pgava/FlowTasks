using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:gettraceforworkflow")]
    public class GetTraceForWorkflowRequest
    {
        [DataMember]
        public Guid[] WorkflowOids { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:gettraceforworkflow")]
    public class GetTraceForWorkflowResponse
    {
        [DataMember]
        public WorkflowTraceInfos Traces { get; set; }
    }

    [CollectionDataContract]
    public class WorkflowTraceInfos : List<WorkflowTraceInfo>
    {
        public WorkflowTraceInfos()
        {
        }

        public WorkflowTraceInfos(IEnumerable<WorkflowTraceInfo> traces)
            : base(traces)
        {
        }
    }

}
