using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class TopicInfo
    {
        [DataMember]
        public int TopicId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public TopicStatusType Status { get; set; }

        [DataMember]
        public TopicMessageInfo Message { get; set; }

        [DataMember]
        public TopicMessageInfo[] Replies { get; set; }
    }
}
