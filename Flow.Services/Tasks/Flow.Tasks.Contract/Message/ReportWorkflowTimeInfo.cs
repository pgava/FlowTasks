using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportworkflowtimeinfo")]
    public class ReportWorkflowTimeInfo
    {
        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public string Workflow { get; set; }

    }
}
