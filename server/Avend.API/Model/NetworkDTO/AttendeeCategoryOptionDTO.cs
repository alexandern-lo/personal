using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryOptionDto
    {
        [DataMember(Name = "option_uid")]
        public Guid? Uid { get; set; }


        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static AttendeeCategoryOptionDto From(AttendeeCategoryOption obj)
        {
            var dto = new AttendeeCategoryOptionDto()
            {
                Uid = obj.Uid,
                Name = obj.Name,
            };

            return dto;
        }

        public void ApplyChangesToModel(AttendeeCategoryOption attendeeOption)
        {
            if (Name != null)
                attendeeOption.Name = Name;
        }
    }
}
