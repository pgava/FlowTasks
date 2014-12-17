using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Tasks.Contract.Message
{
     [DataContract(Namespace = "urn:flowtasks:getworkflowtype")]
    public class GetWorkflowTypeRequest
    {
        [DataMember]
        public DateTime EffectiveDate { get; set; }
    }

     [DataContract(Namespace = "urn:flowtasks:getworkflowtype")]
     public class GetWorkflowTypeResponse
     {
         [DataMember]
         public WorkflowTypeInfos WorkflowTypeInfos { get; set; }

     }

     [CollectionDataContract]
     public class WorkflowTypeInfos : List<WorkflowTypeInfo>
     {
         public WorkflowTypeInfos()
         {
         }

         public WorkflowTypeInfos(IEnumerable<WorkflowTypeInfo> traces)
             : base(traces)
         {
         }
     }

}
