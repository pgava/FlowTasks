using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getusernames")]
    public class GetUserNamesResponse
    {
        [DataMember]
        public IEnumerable<UserInfo> UserNames { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getusernames")]
    public class GetUserNamesRequest
    {
        [DataMember]
        public string NameToSearch { get; set; }
    }
}
