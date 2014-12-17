using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getworkflow")]
    public class GetWorkflowRequest
    {
        [DataMember]
        public string WorkflowId { get; set; }

        [DataMember]
        public string WorkflowCode { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public DateTime? StartedFrom { get; set; }

        [DataMember]
        public DateTime? StartedTo { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Role { get; set; }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getworkflow")]
    public class GetWorkflowResponse
    {
        [DataMember]
        public int TotalWorkflows { get; set; }

        [DataMember]
        public int PageIndex { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public IEnumerable<string> WorkflowCodes { get; set; }

        [DataMember]
        public WorkflowInfos WorkflowInfos { get; set; }

    }

    [CollectionDataContract]
    public class WorkflowInfos : List<WorkflowInfo>
    {
        public WorkflowInfos()
        {
        }

        public WorkflowInfos(IEnumerable<WorkflowInfo> workflows)
            : base(workflows)
        {
        }
    }
}
