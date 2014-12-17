using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportusertaskcountinfo")]
    public class ReportUserTaskCountInfo
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Task { get; set; }

        [DataMember]
        public int Count { get; set; }

    }
}
