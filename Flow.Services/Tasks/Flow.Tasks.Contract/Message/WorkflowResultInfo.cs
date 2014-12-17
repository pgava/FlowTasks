using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class WorkflowResultInfo
    {
        [DataMember]
        public Guid WorkflowOid { get; set; }

        [DataMember]
        public DateTime CompletedOn { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public PropertyInfos OutParameters { get; set; }
    }
}
