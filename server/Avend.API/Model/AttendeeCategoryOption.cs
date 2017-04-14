using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    /// <summary>
    /// One possible value for <see cref="AttendeeCategory"/>
    /// </summary>
    [Table("attendee_category_options")]
    public class AttendeeCategoryOption
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("uid", TypeName = "UniqueIdentifier")]
        public Guid Uid { get; set; }

        [Column("attendee_category_id")]
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public AttendeeCategoryRecord AttendeeCategory { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
