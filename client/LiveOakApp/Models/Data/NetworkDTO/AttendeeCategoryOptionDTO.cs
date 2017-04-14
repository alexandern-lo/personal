using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryOptionDTO
    {
        [DataMember(Name = "option_uid")]
        public string UID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
