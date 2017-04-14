using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class AttendeeCategoryValueDto
    {
        [DataMember(Name = "category_uid")]
        public Guid CategoryUid { get; set; }

        [DataMember(Name = "option_uid")]
        public Guid OptionUid { get; set; }

        [DataMember(Name = "category_name")]
        public string CategoryName { get; set; }

        [DataMember(Name = "option_name")]
        public string OptionName { get; set; }

        public static AttendeeCategoryValueDto From(AttendeeCategoryValue obj)
        {
            var c = obj.Category;
            var dto = new AttendeeCategoryValueDto
            {
                CategoryUid = c?.Uid ?? Guid.Empty,
                CategoryName = c?.Name,
                OptionUid = obj.AttendeeCategoryOption?.Uid ?? Guid.Empty,
                OptionName = obj.AttendeeCategoryOption?.Name,
            };

            return dto;
        }
    }
}