using System;
using System.Runtime.Serialization;

using Avend.API.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Services.Leads.NetworkDTO
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LeadPerformedAction
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "created")]
        Created = 1,

        [EnumMember(Value = "updated")]
        Updated = 2,

        [EnumMember(Value = "deleted")]
        Deleted = 3,
    }

    [DataContract]
    public class LeadRecentActivityDto
    {
        [DataMember(Name = "lead_uid")]
        public Guid? LeadUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "photo_url")]
        public string PhotoUrl { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "event_name")]
        public string EventName { get; set; }

        [DataMember(Name = "performed_action")]
        public LeadPerformedAction? PerformedAction { get; set; }

        [DataMember(Name = "performed_at")]
        public DateTime? PerformedAt { get; set; }

        public static LeadRecentActivityDto From(LeadRecord obj)
        {
            LeadPerformedAction? action;

            if (obj.Deleted)
            {
                action = LeadPerformedAction.Deleted;
            }
            else
            {
                if (obj.CreatedAt == obj.UpdatedAt)
                    action = LeadPerformedAction.Created;
                else
                    action = LeadPerformedAction.Updated;
            }

            var dto = new LeadRecentActivityDto()
            {
                LeadUid = obj.Uid,

                EventUid = obj.Event?.Uid,
                EventName = obj.Event?.Name,

                FirstName = obj.FirstName,
                LastName = obj.LastName,
                PhotoUrl = obj.PhotoUrl,

                PerformedAt = obj.UpdatedAt,
                PerformedAction = action,
            };

            return dto;
        }
    }
}