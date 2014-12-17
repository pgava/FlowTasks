using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getworkflowchildren")]
    public class GetWorkflowChildrenRequest
    {
        [DataMember]
        public string WorkflowOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getworkflowchildren")]
    public class GetWorkflowChildrenResponse
    {
        [DataMember]
        public IEnumerable<string> Children { get; set; }
    }
}
