using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:updateuser")]
    public class UpdateUserRequest
    {
        [DataMember]
        public UserInfo User { get; set; }
    }
}
