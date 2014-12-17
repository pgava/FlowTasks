using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getnexttasksforuser")]
    public class GetNextTasksForUserRequest
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public string SearchFor { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:getnexttasksforuser")]
    [KnownType(typeof(TaskInfo))]
    public class GetNextTasksForUserResponse
    {
        [DataMember]
        public TaskInfo[] Tasks { get; set; }
    }
}
