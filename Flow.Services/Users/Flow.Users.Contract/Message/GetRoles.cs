using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getroles")]
    public class GetRolesResponse
    {
        [DataMember]
        public IEnumerable<RoleInfo> Roles { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getroles")]
    public class GetRolesRequest
    {
        [DataMember]
        public string roleToSearch { get; set; }
    }
}
