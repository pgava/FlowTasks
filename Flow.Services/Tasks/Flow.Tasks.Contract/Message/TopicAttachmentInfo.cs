using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class TopicAttachmentInfo
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public Guid DocumentOid { get; set; }
    }
}
