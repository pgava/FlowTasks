using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:gethandoverusersfortask")]
    public class GetHandOverUsersForTaskRequest
    {
        [DataMember]
        Guid TaskOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:gethandoverusersfortask")]
    public class GetHandOverUsersForTaskResponse
    {
        [DataMember]
        string[] Users { get; set; }
    }
}
