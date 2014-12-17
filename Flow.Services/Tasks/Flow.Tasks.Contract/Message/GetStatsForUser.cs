using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getstatsforuser")]
    public class GetStatsForUserRequest
    {
        [DataMember]
        public string User { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getstatsforuser")]
    [KnownType(typeof(TasksOn))]
    public class GetStatsForUserResponse
    {
        [DataMember]
        public int TaskCompleted { get; set; }

        [DataMember]
        public int TaskToDo { get; set; }

        [DataMember]
        public IEnumerable<TasksOn> TasksCompleted { get; set; }

        [DataMember]
        public IEnumerable<TasksOn> TasksToDo { get; set; }
    }
}
