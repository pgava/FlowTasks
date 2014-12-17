using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:assigntaskto")]
    public class AssignTaskToRequest
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public Guid TaskOid { get; set; }
    }
}
