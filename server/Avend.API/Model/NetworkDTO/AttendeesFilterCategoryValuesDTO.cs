using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract(Name = "attendee_category_filter")]
    public class AttendeesFilterCategoryValuesDTO
    {
        [DataMember(Name = "category_uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "values")]
        public List<Guid> Values { get; set; }
    }
}