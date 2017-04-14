using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryDTO
    {
        public AttendeeCategoryDTO()
        {
            Options = new List<AttendeeCategoryOptionDTO>();
        }

        [DataMember(Name = "category_uid")]
        public string UID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "options")]
        public List<AttendeeCategoryOptionDTO> Options { get; set; }
    }
}
