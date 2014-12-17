using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:gettask")]
    public class GetTaskRequest
    {
        [DataMember]
        public Guid TaskOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:gettask")]
    [KnownType(typeof(TaskInfo))]
    public class GetTaskResponse
    {
        [DataMember]
        public TaskInfo Task { get; set; }
    }
}
