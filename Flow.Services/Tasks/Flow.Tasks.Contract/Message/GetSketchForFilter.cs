using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getsketchforfilter")]
    public class GetSketchForFilterRequest
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IEnumerable<SketchStatusType> Statuses { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:getsketchforfilter")]
    public class GetSketchForFilterResponse
    {
        [DataMember]
        public SketchInfos Sketches { get; set; }
    }

}
