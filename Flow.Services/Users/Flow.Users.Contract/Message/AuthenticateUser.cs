using System.Runtime.Serialization;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:authenticateuser")]
    public class AuthenticateUserRequest
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Password { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:authenticateuser")]
    public class AuthenticateUserResponse
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public bool IsAuthenticated { get; set; }
    }

}
