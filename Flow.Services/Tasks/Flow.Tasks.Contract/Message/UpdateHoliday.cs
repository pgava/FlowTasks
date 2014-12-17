using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:updateholiday")]
    public class UpdateHolidayRequest
    {
        [DataMember]
        public string User{ get; set; }

        [DataMember]
        public int HolidayId { get; set; }

        [DataMember]
        public HolidayStatus Status { get; set; }

        [DataMember]
        public IEnumerable<string> Holiday { get; set; }
    }
}
