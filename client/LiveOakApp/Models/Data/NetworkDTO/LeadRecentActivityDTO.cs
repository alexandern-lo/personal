using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "lead_action")]
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

    [DataContract(Name = "lead_recent_activity")]
    public class LeadRecentActivityDTO
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
    }
}
