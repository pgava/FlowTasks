using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getusersbydomain")]
    public class GetUsersByDomainRequest
    {
        [DataMember]
        public IEnumerable<string> Domains { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getusersbydomain")]
    public class GetUsersByDomainResponse
    {
        [DataMember]
        public IEnumerable<string> Users { get; set; }
    }
}
