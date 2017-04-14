using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class EventAgendaItemDTO
    {
        [DataMember(Name = "agenda_item_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "date")]
        public DateTime? Date { get; set; }

        [DataMember(Name = "start_time")]
        public TimeSpan? StartTime { get; set; }

        [DataMember(Name = "end_time")]
        public TimeSpan? EndTime { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "details_url")]
        public string DetailsUrl { get; set; }

        [DataMember(Name = "location_url")]
        public string LocationUrl { get; set; }

        /// <summary>
        /// Date and time of the record's creation.
        /// </summary>
        /// <value>Date and time of the record's creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Date and time of the record's last update.
        /// </summary>
        /// <value>Date and time of the record's last update.</value>
        [DataMember(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public static EventAgendaItemDTO From(EventAgendaItem obj, Guid eventUid)
        {
            var dto = new EventAgendaItemDTO()
            {
                Uid = obj.Uid,

                EventUid = eventUid,

                Name = obj.Name,
                Description = obj.Description,

                Date = obj.Date,
                StartTime = obj.StartTime,
                EndTime = obj.EndTime,

                Location = obj.Location,

                DetailsUrl = obj.DetailsUrl,
                LocationUrl = obj.LocationUrl,

                CreatedAt = obj.CreatedAt,
                UpdatedAt = obj.UpdatedAt,
            };

            return dto;
        }

        public void ApplyChangesToModel(EventAgendaItem eventAgendaItem)
        {
            if (Name != null)
                eventAgendaItem.Name = Name;

            if (Description != null)
                eventAgendaItem.Description = Description;

            if (Date.HasValue)
                eventAgendaItem.Date = Date.Value;

            if (StartTime.HasValue)
                eventAgendaItem.StartTime = StartTime.Value;

            if (EndTime.HasValue)
                eventAgendaItem.EndTime = EndTime.Value;

            if (Location != null)
                eventAgendaItem.Location = Location;

            if (DetailsUrl != null)
                eventAgendaItem.DetailsUrl = DetailsUrl;

            if (LocationUrl != null)
                eventAgendaItem.LocationUrl = LocationUrl;
        }

    }
}