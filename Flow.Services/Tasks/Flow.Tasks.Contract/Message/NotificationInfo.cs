using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class NotificationInfo
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public Guid TaskOid { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string AssignedToUsers { get; set; }
    }
}
