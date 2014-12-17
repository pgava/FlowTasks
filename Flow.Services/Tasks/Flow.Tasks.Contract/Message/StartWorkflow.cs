using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:startworkflow")]
    [KnownType(typeof(WfProperty))]
    public class 
        StartWorkflowRequest
    {
        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public WfProperty[] WfRuntimeValues { get; set; }

        [DataMember]
        public string ParentWorkflowId { get; set; }

        [DataMember]
        public int CorrelationId { get; set; }

        [DataMember]
        public bool WaitForChild { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:startworkflow")]
    public class WfProperty
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Type { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:startworkflow")]
    public class StartWorkflowResponse
    {
        [DataMember]
        public string WorkflowId { get; set; }
    }

}
