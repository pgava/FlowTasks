using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:addfollowinguser")]
    public class AddFollowingUserRequest
    {
        [DataMember]
        public string Follower { get; set; }

        [DataMember]
        public string Following { get; set; }

    }
}
