using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:updateworkflowparameters")]
    [KnownType(typeof(WfProperty))]
    public class UpdateWorkflowParametersRequest
    {
        [DataMember]
        public string WorkflowOid { get; set; }

        [DataMember]
        public string TaskOid { get; set; }

        [DataMember]
        public WfProperty[] WfRuntimeValues { get; set; }
    }
}
