using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract]
    public class RoleInfo
    {
        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public IEnumerable<string> Users { get; set; }

    }

}
