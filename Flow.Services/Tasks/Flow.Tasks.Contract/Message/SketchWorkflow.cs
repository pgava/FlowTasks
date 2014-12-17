using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:sketchworkflow")]
    public class SketchWorkflowRequest
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string XamlxOid { get; set; }

        [DataMember]
        public string ChangedBy { get; set; }

        [DataMember]
        public SketchStatusType Status { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:sketchworkflow")]
    public class SketchWorkflowResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

}
