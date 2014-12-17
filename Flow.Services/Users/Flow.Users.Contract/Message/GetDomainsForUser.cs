using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getdomainsforuser")]
    public class GetDomainsForUserRequest
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Role { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getdomainsforuser")]
    public class GetDomainsForUserResponse
    {
        [DataMember]
        public IEnumerable<string> Domains { get; set; }
    }
}
