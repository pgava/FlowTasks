using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class PropertyInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Type { get; set; }

    }

    [CollectionDataContract]
    public class PropertyInfos : List<PropertyInfo>
    {
        public PropertyInfos()
        {
        }

        public PropertyInfos(IEnumerable<PropertyInfo> properties)
            : base(properties)
        {
        }

    }


}
