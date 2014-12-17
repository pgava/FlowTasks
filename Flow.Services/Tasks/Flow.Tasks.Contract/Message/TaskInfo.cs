using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class TaskInfo
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public Guid TaskOid { get; set; }

        [DataMember]
        public int TaskCorrelationId { get; set; }

        [DataMember]
        public string TaskCode { get; set; }

        [DataMember]
        public string UiCode { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DefaultResult { get; set; }

        [DataMember]
        public DateTime? ExpiryDate { get; set; }

        [DataMember]
        public string ExpiresIn { get; set; }

        [DataMember]
        public DateTime? ExpiresWhen { get; set; }

        [DataMember]
        public string HandOverUsers { get; set; }

        [DataMember]
        public string AssignedToUsers { get; set; }

        [DataMember]
        public string AcceptedBy { get; set; }
    }

    [DataContract]
    public class TasksOn
    {

        [DataMember]
        public string Date { get; set; }

        [DataMember]
        public int Counter { get; set; }

    }

}
