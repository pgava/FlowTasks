using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getrolesforuser")]
    public class GetRolesForUserRequest
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public bool IsPrimaryRole { get; set; }

    }

    [DataContract(Namespace = "urn:flowtasks:getrolesforuser")]
    public class GetRolesForUserResponse
    {
        [DataMember]
        public IEnumerable<string> Roles { get; set; }
    }
}
