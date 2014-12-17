using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Docs.Data.Core
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        [MaxLength(100)]
        public string FileName { get; set; }
        public byte[] DataField { get; set;}
        public int Partial { get; set; }

        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public Guid? OidDocument { get; set; }

        public DateTime? DateCreated { get; set; }

    }
}
