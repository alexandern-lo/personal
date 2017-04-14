using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class AgendaItemDTO
    {
        [DataMember(Name = "agenda_item_uid")]
        public string UID { get; set; }

        [DataMember(Name = "event_uid")]
        public string EventUid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "start_time")]
        public TimeSpan StartTime { get; set; }

        [DataMember(Name = "end_time")]
        public TimeSpan EndTime { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "details_url")]
        public string DetailsUrl { get; set; }

        [DataMember(Name = "location_url")]
        public string LocationUrl { get; set; }
    }
}