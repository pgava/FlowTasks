using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:handovertaskto")]
    public class HandOverTaskToRequest
    {
        [DataMember]
        string User { get; set; }

        [DataMember]
        Guid TaskOid { get; set; }
    }
}
