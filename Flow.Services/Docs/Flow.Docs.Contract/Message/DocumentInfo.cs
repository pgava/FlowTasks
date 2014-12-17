using System.Runtime.Serialization;
using System;

namespace Flow.Docs.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:documentinfo")]
    public class DocumentInfo
    {
        [DataMember]
        public string DocumentName { get; set; }
        
        [DataMember]
        public Guid OidDocument { get; set; }

        [DataMember]
        public Guid PreviousOid { get; set; }
        
        [DataMember]
        public string Description { get; set; }
        
        [DataMember]
        public string Owner { get; set; }
        
        [DataMember]
        public string Path { get; set; }
        
        [DataMember]
        public int Version { get; set; }
        
        [DataMember]
        public string MimeType { get; set; }
        
        [DataMember]
        public string FileHash { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:documentinfo")]
    public class DocumentInfoRequest
    {
        [DataMember]
        public Guid[] OidDocuments { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:documentinfo")]
    public class DocumentInfoResponse
    {
        [DataMember]
        public DocumentInfo[] DocumentInfos { get; set; }
    }

}
