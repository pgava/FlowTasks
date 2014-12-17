using System;
using System.Runtime.Serialization;

namespace Flow.Docs.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:uploaddocument")]
    public class UploadDocumentRequest
    {
        [DataMember]
        public Guid OidDocument { get; set; }

        [DataMember]
        public Guid PreviousOid { get; set; }

        [DataMember]
        public string DocumentName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Owner { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public byte[] DataField { get; set; }

        [DataMember]
        public int ChunkSize { get; set; }

        [DataMember]
        public int ChunkNumber { get; set; }

        [DataMember]
        public int ChunkTotal { get; set; }

        [DataMember]
        public string MimeType { get; set; }

        [DataMember]
        public string FileHash { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        [DataMember]
        public DocumentUploadMode Mode { get; set; }
    }

    public enum DocumentUploadMode
    {
        Overwrite,
        NewVersion,
        SpecifiedVersion
    }


    [DataContract(Namespace = "urn:flowtasks:uploaddocument")]
    public class UploadDocumentResponse
    {
        [DataMember]
        public Guid OidDocument { get; set; }
    }

}
