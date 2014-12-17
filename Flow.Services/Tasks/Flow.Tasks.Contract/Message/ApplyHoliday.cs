using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:applyholiday")]
    public class ApplyHolidayRequest
    {
        [DataMember]
        public string User{ get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public IEnumerable<string> Holiday { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:applyholiday")]
    public class ApplyHolidayResponse
    {
        [DataMember]
        public int HolidayId { get; set; }
    }
}
