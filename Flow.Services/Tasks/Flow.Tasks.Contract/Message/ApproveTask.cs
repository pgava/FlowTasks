using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:approvetask")]
    public class ApproveTaskRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public int CorrelationId { get; set; }

        [DataMember]
        public string TaskId { get; set; }

        [DataMember]
        public string TaskCode { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public PropertyInfos Parameters { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:approvetask")]
    public class ApproveTaskResponse
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public string TaskId { get; set; }
    }
}
