using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    [Table("event_lead_goals")]
    public class EventUserGoalsRecord : BaseUserDependentRecord
    {
        [Key]
        [Column("event_lead_goal_id")]
        public override long Id { get; set; }

        [Column("event_lead_goal_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventRecord Event { get; set; }

        [Column("leads_goal")]
        public int LeadsGoal { get; set; }

        [Column("leads_acquired")]
        public int LeadsAcquired { get; set; }
    }
}
