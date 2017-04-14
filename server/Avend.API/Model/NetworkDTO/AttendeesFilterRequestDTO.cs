using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract(Name = "attendee_filter")]
    public class AttendeesFilterRequestDTO
    {
        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "categories")]
        public List<AttendeesFilterCategoryValuesDTO> Categories { get; set; }
    }
}