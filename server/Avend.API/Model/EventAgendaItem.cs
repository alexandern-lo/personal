using System;
using System.ComponentModel.DataAnnotations.Schema;

using Avend.API.Infrastructure.SearchExtensions;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    [Table("event_agenda_items")]
    [DefaultFilter("Name", "Description", "Location")]
    public class EventAgendaItem : BaseRecord
    {
        [Column("event_agenda_item_id")]
        public override long Id { get; set; }

        [Column("event_agenda_item_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventRecord EventRecord { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("start_time_ticks")]
        public long StartTimeTicks { get; set; }

        [NotMapped]
        public TimeSpan StartTime
        {
            get { return TimeSpan.FromTicks(StartTimeTicks); }
            set { StartTimeTicks = value.Ticks; }
        }

        [Column("end_time_ticks")]
        public long EndTimeTicks { get; set; }

        [NotMapped]
        public TimeSpan EndTime
        {
            get { return TimeSpan.FromTicks(EndTimeTicks); }
            set { EndTimeTicks = value.Ticks; }
        }

        [Column("location")]
        public string Location { get; set; }

        [Column("details_url")]
        public string DetailsUrl { get; set; }

        [Column("location_url")]
        public string LocationUrl { get; set; }
    }
}