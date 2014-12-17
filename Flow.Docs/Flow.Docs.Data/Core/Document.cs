using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Docs.Data.Core
{
    public class Document
    {
        public int DocumentId { get; set; }
        [Required]
        public Guid OidDocument { get; set; }
        [MaxLength(100)]
        public string DocumentName { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
        [MaxLength(16)]
        public string Owner { get; set; }
        [MaxLength(256)]
        public string Path { get; set; }
        public int Version { get; set; }
        [MaxLength(200)]
        public string MimeType { get; set; }
        [MaxLength(20)]
        public string FileHash { get; set; }
        public long FileSize { get; set; }

        public int? DocumentPreviousId { get; set; }
        public Document DocumentPrevious { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
    }
}
