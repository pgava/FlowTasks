using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:searchfortasks")]
    public class SearchForTasksRequest
    {
        [DataMember]
        public string TaskCode{ get; set; }

        [DataMember]
        public string AcceptedBy { get; set; }

        [DataMember]
        public PropertyInfos Properties { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:searchfortasks")]
    [KnownType(typeof(TaskInfo))]
    public class SearchForTasksResponse
    {
        [DataMember]
        public TaskInfo[] Tasks { get; set; }
    }
}
