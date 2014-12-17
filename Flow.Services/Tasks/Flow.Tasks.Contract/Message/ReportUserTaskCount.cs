using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportusertaskcount")]
    public class ReportUserTaskCountRequest
    {
        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public IEnumerable<string> Tasks { get; set; }

        [DataMember]
        public IEnumerable<string> Users { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:reportusertaskcount")]
    public class ReportUserTaskCountResponse
    {
        [DataMember]
        public ReportUserTaskCountInfos Report { get; set; }
    }

    [CollectionDataContract]
    public class ReportUserTaskCountInfos : List<ReportUserTaskCountInfo>
    {
        public ReportUserTaskCountInfos()
        {
        }

        public ReportUserTaskCountInfos(IEnumerable<ReportUserTaskCountInfo> report)
            : base(report)
        {
        }
    }

}
