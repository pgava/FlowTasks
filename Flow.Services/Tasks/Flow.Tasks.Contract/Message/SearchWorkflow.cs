using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:searchworkflows")]
    public class SearchWorkflowsRequest
    {
        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public PropertyInfos Properties { get; set; }
    }
    

    [DataContract(Namespace = "urn:flowtasks:searchworkflows")]
    public class SearchWorkflowsResponse
    {
        [DataMember]
        public WorkflowInfos WorkflowInfos { get; set; }
    }

}
