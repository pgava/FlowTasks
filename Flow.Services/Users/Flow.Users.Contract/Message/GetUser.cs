using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getuser")]
    public class GetUserRequest
    {
        [DataMember]
        public string User { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getuser")]
    public class GetUserResponse
    {
        [DataMember]
        public UserInfo User { get; set; }
    }
}
