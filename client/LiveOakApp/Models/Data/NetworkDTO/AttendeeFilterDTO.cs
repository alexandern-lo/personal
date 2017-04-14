using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StudioMobile;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "attendee_filter")]
    public class AttendeeFilterDTO
    {
        [DataMember(Name = "query")]
        public string Query { get; set; }

        [DataMember(Name = "categories")]
        public List<AttendeeCategoryFilterDTO> CategoryFilters { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as AttendeeFilterDTO;
            if (other == null) return false;

            if (!Equals(Query, other.Query)) return false;
            if (!CategoryFilters.HasSameObjects(other.CategoryFilters)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Query.GetHashCode() ^
                      CategoryFilters.Aggregate(0, (memo, item) => memo ^ item.GetHashCode());
        }
    }
}
