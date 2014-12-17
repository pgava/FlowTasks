using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flow.Docs.Data.Core
{
    public class Chunk
    {
        public int ChunkId { get; set; }
        public int ChunkNo { get; set; }
        public string Content { get; set; }
        public string CheckSum { get; set; }

        public int AttachmentId { get; set; }
        public virtual Attachment Attachment { get; set; }
    }
}
