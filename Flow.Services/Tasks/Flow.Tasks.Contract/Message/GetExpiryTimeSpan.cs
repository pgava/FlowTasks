using System.Runtime.Serialization;
using System;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getexpirytimespan")]
    public class GetExpiryTimeSpanRequest
    {
        [DataMember]
        public DateTime? ExpiresWhen { get; set; }

        [DataMember]
        public string ExpiresIn { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getexpirytimespan")]
    public class GetExpiryTimeSpanResponse
    {
        [DataMember]
        public TimeSpan Expires { get; set; }
    }
}
