using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Avend.API.Infrastructure.SearchExtensions;
using Newtonsoft.Json;

namespace Avend.API.Model
{
    public enum EventType
    {
        Personal,
        Conference
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("events")]
    [DefaultFilter("Name")]
    public class EventRecord: BaseRecord, IDeletable
    {
        public static readonly string EventTypePersonal = "personal";
        public static readonly string EventTypeConference = "conference";

        /// <summary>
        /// Initializes a new instance of the <see cref="LeadRecord" /> class.
        /// </summary>
        public EventRecord()
        {
            AttendeeCategories = new List<AttendeeCategoryRecord>();  
            Questions = new List<EventQuestionRecord>();
        }

        /// <summary>
        /// Bigint identifier representing the specific event.
        /// </summary>
        /// <value>Bigint identifier representing the specific event.</value>
        [Key]
        [Column("event_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific event.
        /// </summary>
        /// <value>Unique identifier representing the specific event.</value>
        [Column("event_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Event type. Can only be conference or personal at the moment.
        /// </summary>
        /// <value>Event type. Can only be conference or personal at the moment.</value>
        [Column("event_type")]
        [MaxLength(20)]
        public string Type { get; set; }

        [Column("owner_id")]
        public long? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public SubscriptionMember Owner { get; set; }

        [Column("subscription_id")]
        public long? SubscriptionId { get; set; }

        [ForeignKey("SubscriptionId")]
        public SubscriptionRecord Subscription { get; set; }

        /// <summary>
        /// URL for event logo image.
        /// </summary>
        /// <value>URL for event logo image.</value>
        [Column("logo_url")]
        [MaxLength(1024)]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Event website URL.
        /// </summary>
        /// <value>Event website URL.</value>
        [Column("website_url")]
        [MaxLength(1024)]
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        /// <value>Event name</value>
        [Column("name")]
        [MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Event venue name
        /// </summary>
        /// <value>Event venue name</value>
        [Column("venue_name")]
        [MaxLength(200)]
        public string VenueName { get; set; }

        /// <summary>
        /// Start date, UTC
        /// </summary>
        /// <value>Start date, UTC</value>
        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date, UTC
        /// </summary>
        /// <value>End date, UTC</value>
        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        /// <value>Address</value>
        [Column("address")]
        [MaxLength(500)]
        public string Address { get; set; }

        /// <summary>
        /// Zip/Postal Code
        /// </summary>
        /// <value>Zip/Postal Code</value>
        [Column("zip_code")]
        [MaxLength(200)]
        public string ZipCode { get; set; }

        /// <summary>
        /// City name
        /// </summary>
        /// <value>City name</value>
        [Column("city")]
        [MaxLength(200)]
        public string City { get; set; }

        /// <summary>
        /// State/Region/Province name
        /// </summary>
        /// <value>State/Region/Province name</value>
        [Column("state")]
        [MaxLength(200)]
        public string State { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        /// <value>Country name</value>
        [Column("country")]
        [MaxLength(200)]
        public string Country { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        /// <value>Industry</value>
        [Column("industry")]
        [MaxLength(200)]
        public string Industry { get; set; }

        [Column("description")]
        [MaxLength(2048)]
        public string Description { get; set; }

        /// <summary>
        /// Attendee categories 
        /// </summary>
        /// <value>List of attendee categories</value>
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<AttendeeCategoryRecord> AttendeeCategories { get; set; }

        /// <summary>
        /// List of event questions with answers.
        /// </summary>
        /// <value>List of event questions with answers.</value>
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public List<EventQuestionRecord> Questions { get; set; }

        [Column("recurring")]
        public bool Recurring { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventRecord {\n");
            sb.Append("  EventUid: ").Append(Uid).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  VenueName: ").Append(VenueName).Append("\n");
            sb.Append("  StartDate: ").Append(StartDate).Append("\n");
            sb.Append("  EndDate: ").Append(EndDate).Append("\n");
            sb.Append("  Address: ").Append(Address).Append("\n");
            sb.Append("  ZipCode: ").Append(ZipCode).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  State: ").Append(State).Append("\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  Industry: ").Append(Industry).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
