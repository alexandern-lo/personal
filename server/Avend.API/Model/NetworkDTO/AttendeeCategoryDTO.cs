using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryDto
    {
        public AttendeeCategoryDto()
        {
            Options = new List<AttendeeCategoryOptionDto>();
        }

        [DataMember(Name = "category_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid EventUid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "options")]
        public List<AttendeeCategoryOptionDto> Options { get; set; }

        public static AttendeeCategoryDto From(AttendeeCategoryRecord obj, Guid eventUid)
        {
            var dto = new AttendeeCategoryDto()
            {
                Uid = obj.Uid,
                EventUid = eventUid,
                Name = obj.Name,
                Options = (obj.Options ?? new List<AttendeeCategoryOption>()).Select(AttendeeCategoryOptionDto.From).ToList(),
            };

            return dto;
        }

        public void ApplyChangesToModel(AttendeeCategoryRecord attendeeCategory)
        {
            if (Name != null)
                attendeeCategory.Name = Name;

            if (Options != null)
                attendeeCategory.Options = Options.Select(x =>
                {
                    var option = new AttendeeCategoryOption()
                    {
                        Uid = Guid.NewGuid(),
                        AttendeeCategory = attendeeCategory,
                    };
                    x.ApplyChangesToModel(option);
                    return option;
                }).ToList();
        }
    }
}
