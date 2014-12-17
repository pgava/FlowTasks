using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
     [DataContract(Namespace = "urn:flowtasks:getexpirydatetime")]
    public class GetExpiryDatetimeRequest
    {
        [DataMember]
        public DateTime? ExpiresWhen { get; set; }

        [DataMember]
        public string ExpiresIn { get; set; }
    }

     [DataContract(Namespace = "urn:flowtasks:getexpirydatetime")]
     public class GetExpiryDatetimeResponse
     {
         [DataMember]
         public DateTime? Expires { get; set; }

     }

}
