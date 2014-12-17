using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:workflowevent")]
    public class WorkflowEventRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public string CurrentId { get; set; }

        [DataMember]
        public int CorrelationId { get; set; }

        [DataMember]
        public string Event { get; set; }

        [DataMember]
        public string Result { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:workflowevent")]
    public class WorkflowEventResponse
    {
        [DataMember]
        public string Ack { get; set; }
    }

}
