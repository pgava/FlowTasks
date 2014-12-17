using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:completetask")]
    public class CompleteTaskRequest
    {
        [DataMember]
        public string TaskId { get; set; }

        [DataMember]
        public string Result{ get; set; }

        [DataMember]
        public string User{ get; set; }
    }
}
