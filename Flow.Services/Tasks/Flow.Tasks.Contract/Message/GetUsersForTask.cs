using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getusersfortask")]
    public class GetUsersForTaskRequest
    {
        [DataMember]
        public Guid TaskOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getusersfortask")]
    public class GetUsersForTaskResponse
    {
        [DataMember]
        public string[] Users { get; set; }
    }

}
