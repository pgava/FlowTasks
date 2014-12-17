using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
     [DataContract(Namespace = "urn:flowtasks:gettopiccount")]
    public class GetTopicCountRequest
    {
        [DataMember]
        public string User { get; set; }
    }

     [DataContract(Namespace = "urn:flowtasks:gettopiccount")]
     public class GetTopicCountResponse
     {
         [DataMember]
         public int Count { get; set; }

     }

}
