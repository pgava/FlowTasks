using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:reporttasktime")]
    public class ReportTaskTimeRequest
    {
        [DataMember]
        public DateTime? Start { get; set; }

        [DataMember]
        public DateTime? End { get; set; }

        [DataMember]
        public IEnumerable<string> Tasks { get; set; }

        [DataMember]
        public IEnumerable<string> Workflows { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:reporttasktime")]
    public class ReportTaskTimeResponse
    {
        [DataMember]
        public ReportTaskTimeInfos report { get; set; }
    }

    [CollectionDataContract]
    public class ReportTaskTimeInfos : List<ReportTaskTimeInfo>
    {
        public ReportTaskTimeInfos()
        {
        }

        public ReportTaskTimeInfos(IEnumerable<ReportTaskTimeInfo> report)
            : base(report)
        {
        }
    }

}
