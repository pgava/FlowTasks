using System.Runtime.Serialization;

namespace Flow.Users.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:isvaliduser")]
    public class IsValidUserRequest
    {
        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string User { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:isvaliduser")]
    public class IsValidUserResponse
    {
        [DataMember]
        public bool IsValid { get; set; }
    }
}
