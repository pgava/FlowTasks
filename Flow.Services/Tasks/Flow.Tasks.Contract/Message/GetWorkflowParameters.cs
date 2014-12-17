using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getworkflowparameters")]
    public class GetWorkflowParametersRequest
    {
        [DataMember]
        public string WorkflowOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getworkflowparameters")]
    public class GetWorkflowParametersResponse
    {
        [DataMember]
        public PropertyInfos Properties { get; set; }
    }

}
