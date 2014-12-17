using System;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract(Namespace = "urn:flowtasks:getpropertiesfortask")]
    public class GetPropertiesForTaskRequest
    {
        [DataMember]
        public Guid TaskOid { get; set; }
    }

    [DataContract(Namespace = "urn:flowtasks:getpropertiesfortask")]
    public class GetPropertiesForTaskResponse
    {
        [DataMember]
        public PropertyInfos Properties { get; set; }
    }

}
