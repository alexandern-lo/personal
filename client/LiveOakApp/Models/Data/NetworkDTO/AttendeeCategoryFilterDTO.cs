using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using StudioMobile;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "attendee_category_filter")]
    public class AttendeeCategoryFilterDTO
    {
        [DataMember(Name = "category_uid")]
        public string Uid { get; set; }

        [DataMember(Name = "values")]
        public List<string> Values { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as AttendeeCategoryFilterDTO;
            if (other == null) return false;

            if (!Equals(Uid, other.Uid)) return false;
            if (!Values.HasSameObjects(other.Values)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Uid.GetHashCode() ^
                      Values.Aggregate(0, (memo, item) => memo ^ item.GetHashCode());
        }
    }
}
