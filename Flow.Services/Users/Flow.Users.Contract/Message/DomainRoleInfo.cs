using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract]
    public class DomainRoleInfo
    {
        [DataMember]
        public IEnumerable<string> Domanis { get; set; }

        [DataMember]
        public IEnumerable<string> Roles { get; set; }
    }

}
