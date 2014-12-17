using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:createnotification")]
    public class CreateNotificationRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public NotificationInfo NotificationInfo{ get; set; }
    }
}
