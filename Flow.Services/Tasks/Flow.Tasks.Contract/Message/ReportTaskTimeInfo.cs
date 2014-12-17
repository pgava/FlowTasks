using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reporttasktimeinfo")]
    public class ReportTaskTimeInfo
    {
        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string Task { get; set; }

        [DataMember]
        public string Workflow { get; set; }

    }
}
