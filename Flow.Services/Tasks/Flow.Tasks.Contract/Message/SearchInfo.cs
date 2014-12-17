using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Flow.Tasks.Contract.Message
{
    [DataContract]
    public class SearchInfo
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public int TopicId { get; set; }

        [DataMember]
        public int Rank { get; set; }
    }

    [CollectionDataContract]
    public class SearchInfos : List<SearchInfo>
    {
        public SearchInfos()
        {
        }

        public SearchInfos(IEnumerable<SearchInfo> searches)
            : base(searches)
        {
        }

    }


}
