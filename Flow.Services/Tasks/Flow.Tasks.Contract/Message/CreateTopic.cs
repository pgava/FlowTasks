using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:createtopic")]
    [KnownType(typeof(TopicAttachmentInfo))]
    public class CreateTopicRequest
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public IEnumerable<TopicAttachmentInfo> Attachments { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:createtopic")]
    public class CreateTopicResponse
    {
        [DataMember]
        public int TopicId { get; set; }
    }

}
