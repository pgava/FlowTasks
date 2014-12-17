using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getholidayforuser")]
    public class GetHolidayForUserRequest
    {
        [DataMember]
        public string User{ get; set; }

        [DataMember]
        public int HolidayId { get; set; }

        [DataMember]
        public int Year { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:getholidayforuser")]
    public class GetHolidayForUserResponse
    {
        [DataMember]
        public IEnumerable<HolidayInfo> Holidays { get; set; }
    }

}
