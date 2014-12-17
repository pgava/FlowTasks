using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class SketchInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid XamlxOid { get; set; }

        [DataMember]
        public string ChangedBy { get; set; }

        [DataMember]
        public string Status { get; set; }
    }

    [CollectionDataContract]
    public class SketchInfos : List<SketchInfo>
    {
        public SketchInfos()
        {
        }

        public SketchInfos(IEnumerable<SketchInfo> sketches)
            : base(sketches)
        {
        }

    }
}
