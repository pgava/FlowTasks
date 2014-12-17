using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:searchfortopic")]
    public class SearchForTopicsRequest
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Pattern { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:searchfortopic")]
    public class SearchForTopicsResponse
    {
        [DataMember]
        public SearchInfos Result { get; set; }
    }
}
