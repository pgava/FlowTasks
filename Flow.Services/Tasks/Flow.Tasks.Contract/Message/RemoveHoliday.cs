using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:removeholiday")]
    public class RemoveHolidayRequest
    {
        [DataMember]
        public string User{ get; set; }

        [DataMember]
        public int HolidayId { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:removeholiday")]
    public class RemoveHolidayResponse
    {
        [DataMember]
        public bool IsRemoved { get; set; }
    }
}
