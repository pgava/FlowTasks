using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:createworkflowrequest")]
    public class CreateWorkflowRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public string ParentWorkflowId { get; set; }

        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public PropertyInfos PropertyInfos { get; set; }

    }
}
