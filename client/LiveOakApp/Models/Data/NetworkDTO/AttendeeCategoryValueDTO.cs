using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryValueDTO
    {
        [DataMember(Name = "value_uid")]
        public string UID { get; set; }

        [DataMember(Name = "category_uid")]
        public string CategoryUID { get; set; }

        [DataMember(Name = "option_uid")]
        public string OptionUID { get; set; }

        [DataMember(Name = "category_name")]
        public string CategoryName { get; set; }

        [DataMember(Name = "option_name")]
        public string OptionName { get; set; }
    }
}
