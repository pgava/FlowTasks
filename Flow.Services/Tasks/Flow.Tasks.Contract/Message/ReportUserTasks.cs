using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportusertasks")]
    public class ReportUserTasksRequest
    {
        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public IEnumerable<string> Users { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:reportusertasks")]
    public class ReportUserTasksResponse
    {
        [DataMember]
        public ReportUserTasksInfos Report { get; set; }
    }

    [CollectionDataContract]
    public class ReportUserTasksInfos : List<ReportUserTasksInfo>
    {
        public ReportUserTasksInfos()
        {
        }

        public ReportUserTasksInfos(IEnumerable<ReportUserTasksInfo> report)
            : base(report)
        {
        }
    }

}
