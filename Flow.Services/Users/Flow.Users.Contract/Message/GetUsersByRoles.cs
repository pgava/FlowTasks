using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getusersbyroles")]
    public class GetUsersByRolesRequest
    {
        [DataMember]
        public IEnumerable<string> Roles { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getusersbyroles")]
    public class GetUsersByRolesResponse
    {
        [DataMember]
        public IEnumerable<string> Users { get; set; }
    }
}
