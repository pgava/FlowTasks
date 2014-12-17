using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class HolidayInfo
    {
        [DataMember]
        public IEnumerable<string> Holiday { get; set; }

        [DataMember]
        public HolidayStatus Status { get; set; }

        [DataMember]
        public string Type { get; set; }

    }

    public enum HolidayStatus
    {
        R, // rejected
        S, // submitted
        A // approved
        
    }
}
