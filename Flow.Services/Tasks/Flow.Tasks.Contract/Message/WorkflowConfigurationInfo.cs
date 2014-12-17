using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class WorkflowConfigurationInfo
    {
        [DataMember]
        public string ServiceEndPoint { get; set; }

        [DataMember]
        public string ServiceUrl { get; set; }

        [DataMember]
        public string BindingConfiguration { get; set; }

        [DataMember]
        public string ServiceDefinition { get; set; }
    }
}
