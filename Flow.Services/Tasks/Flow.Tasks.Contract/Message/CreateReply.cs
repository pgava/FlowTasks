using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:createreply")]
    [KnownType(typeof(TopicAttachmentInfo))]
    public class CreateReplyRequest
    {
        [DataMember]
        public int TopicId { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public IEnumerable<TopicAttachmentInfo> Attachments { get; set; }
    }

}
