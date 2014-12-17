using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class TopicMessageInfo
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string From { get; set; }

        [DataMember]
        public string ImageUrl { get; set; }

        [DataMember]
        public string To { get; set; }

        [DataMember]
        public DateTime When { get; set; }

        [DataMember]
        public TopicStatusType Status { get; set; }

        [DataMember]
        public IEnumerable<TopicAttachmentInfo> Attachments { get; set; }

    }
}
