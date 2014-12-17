using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reportworkflowtime")]
    public class ReportWorkflowTimeRequest
    {
        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public IEnumerable<string> Workflows { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:reportworkflowtime")]
    public class ReportWorkflowTimeResponse
    {
        [DataMember]
        public ReportWorkflowTimeInfos Report { get; set; }
    }

    [CollectionDataContract]
    public class ReportWorkflowTimeInfos : List<ReportWorkflowTimeInfo>
    {
        public ReportWorkflowTimeInfos()
        {
        }

        public ReportWorkflowTimeInfos(IEnumerable<ReportWorkflowTimeInfo> report)
            : base(report)
        {
        }
    }

}
