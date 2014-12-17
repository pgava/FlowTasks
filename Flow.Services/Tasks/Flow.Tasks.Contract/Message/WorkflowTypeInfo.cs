using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class WorkflowTypeInfo
    {
        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Description { get; set; }

    }
}
