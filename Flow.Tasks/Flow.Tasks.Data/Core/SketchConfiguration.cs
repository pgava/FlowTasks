using System;
using System.ComponentModel.DataAnnotations;

namespace Flow.Tasks.Data.Core
{
    public class SketchConfiguration
    {
        public int SketchConfigurationId { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public Guid XamlxOid { get; set; }
        public DateTime LastSavedOn { get; set; }
        [MaxLength(16)]
        public string ChangedBy { get; set; }

        public int SketchStatusId { get; set; }
        public SketchStatus SketchStatus { get; set; }

    }
}
