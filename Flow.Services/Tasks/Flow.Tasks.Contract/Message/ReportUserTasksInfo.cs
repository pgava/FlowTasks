using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportusertasksinfo")]
    public class ReportUserTasksInfo
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public int TaskNo { get; set; }

    }
}
