using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    /// <summary>
    /// Event attendee category (like B2B, B2C) See also <seealso cref="AttendeeCategoryOption"/>.
    /// </summary>
    [Table("attendee_categories")]
    public class AttendeeCategoryRecord : IDeletable
    {
        public AttendeeCategoryRecord()
        {
            Options = new List<AttendeeCategoryOption>();
        }

        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("uid", TypeName = "UniqueIdentifier")]
        public Guid Uid { get; set; }

        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventRecord EventRecord { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<AttendeeCategoryOption> Options { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }
    }
}
