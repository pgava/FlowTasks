using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:gettopicsforuser")]
    public class GetTopicsForUserRequest
    {
        [DataMember]
        public int TopicId { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public DateTime Start { get; set; }

        [DataMember]
        public DateTime End { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public bool WithReplies { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:gettopicsforuser")]
    public class GetTopicsForUserResponse
    {
        [DataMember]
        public TopicInfos Topics { get; set; }
    }

    [CollectionDataContract]
    public class TopicInfos : List<TopicInfo>
    {
        public TopicInfos()
        {
        }

        public TopicInfos(IEnumerable<TopicInfo> topics)
            : base(topics)
        {
        }
    }

}
