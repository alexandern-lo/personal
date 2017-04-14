using System;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

namespace Avend.API.Model
{
    /// <summary>
    /// Specific value for attendee category, connects category, option and attendee together. 
    /// </summary>
    [Table("attendee_category_values")]
    public class AttendeeCategoryValue
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("attendee_id")]
        public long AttendeeId { get; set; }

        [ForeignKey("AttendeeId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public AttendeeRecord Attendee { get; set; }

        [Column("attendee_category_id")]
        public long? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public AttendeeCategoryRecord Category { get; set; }

        [Column("attendee_category_option_id")]
        public long? CategoryOptionId { get; set; }

        [ForeignKey("CategoryOptionId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public AttendeeCategoryOption AttendeeCategoryOption { get; set; }

        [Column("category_value")]
        public string CategoryValue { get; set; }

        [Column("option_value")]
        public string OptionValue { get; set; }
    }
}