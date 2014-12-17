using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:createtask")]
    public class CreateTaskRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public TaskInfo TaskInfo{ get; set; }

        [DataMember]
        public PropertyInfos Properties { get; set; }
    }
}
