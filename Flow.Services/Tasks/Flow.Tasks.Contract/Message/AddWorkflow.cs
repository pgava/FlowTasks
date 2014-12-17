using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:addworkflow")]
    public class AddWorkflowRequest
    {
        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string ServiceUrl { get; set; }

        [DataMember]
        public string BindingConfiguration { get; set; }

        [DataMember]
        public string ServiceEndpoint { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:addworkflow")]
    public class AddWorkflowResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

}
