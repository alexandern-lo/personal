using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Avend.API.Infrastructure.SearchExtensions;
using Newtonsoft.Json;

namespace Avend.API.Model
{
    [Table("attendees")]
    [DefaultFilter("FirstName", "LastName", "Country")]
    public class AttendeeRecord
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("uid", TypeName = "UniqueIdentifier")]
        public Guid Uid { get; set; }

        [Column("event_id")]
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public EventRecord EventRecord { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("avatar_url")]
        public string AvatarUrl { get; set; }

        [Column("country")]
        public string Country { get; set; }

        [Column("state")]
        public string State { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("zipcode")]
        public string ZipCode { get; set; }

        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<AttendeeCategoryValue> Values { get; set; } = new List<AttendeeCategoryValue>();
    }
}