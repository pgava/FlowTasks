using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:givebacktask")]
    public class GiveBackTaskRequest
    {
        [DataMember]
        public Guid TaskOid { get; set; }
    }


}
