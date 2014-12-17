using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getdomainroleforuser")]
    public class GetDomainRoleForUserRequest
    {
        [DataMember]
        public string User { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getdomainroleforuser")]
    public class GetDomainRoleForUserResponse
    {
        [DataMember]
        public DomainRoleInfo DomainRole { get; set; }
    }
}
