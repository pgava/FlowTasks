using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class WorkflowTraceInfo
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public string When { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Avatar { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}
