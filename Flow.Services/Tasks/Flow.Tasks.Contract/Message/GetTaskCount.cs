using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
     [DataContract(Namespace = "urn:flowtasks:gettaskcount")]
    public class GetTaskCountRequest
    {
        [DataMember]
        public string User { get; set; }
    }

     [DataContract(Namespace = "urn:flowtasks:gettaskcount")]
     public class GetTaskCountResponse
     {
         [DataMember]
         public int Count { get; set; }

     }

}
