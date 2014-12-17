using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getrepliesforuser")]
    public class GetRepliesForUserRequest
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
        public RepliesShowType ShowType { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getrepliesforuser")]
    public class GetRepliesForUserResponse
    {
        [DataMember]
        public bool HasOldReplies { get; set; }

        [DataMember]
        public TopicMessageInfos Replies { get; set; }
    }

    [CollectionDataContract]
    public class TopicMessageInfos : List<TopicMessageInfo>
    {
        public TopicMessageInfos()
        {
        }

        public TopicMessageInfos(IEnumerable<TopicMessageInfo> replies)
            : base(replies)
        {
        }
    }

}
