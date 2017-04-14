using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Avend.API.Infrastructure.SearchExtensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
    /// <summary>
    /// Enumeration for all possible subscrption services we support.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LeadQualification
    {
        [EnumMember(Value = "cold")]
        Cold = 0,

        [EnumMember(Value = "warm")]
        Warm = 1,

        [EnumMember(Value = "hot")]
        Hot = 2,
    }

    /// <summary>
    /// 
    /// </summary>
    [Table("leads")]
    [DefaultFilter("FirstName", "LastName", "Country")]
    public class LeadRecord : BaseRecord, IDeletable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeadRecord" /> class.
        /// </summary>
        public LeadRecord()
        {
            Emails = new List<LeadEmail>();
            Phones = new List<LeadPhone>();

            QuestionAnswers = new List<LeadQuestionAnswer>();
            ExportStatuses = new List<LeadExportStatus>();
        }

        /// <summary>
        /// Unique identifier representing the specific lead.
        /// </summary>
        /// <value>Unique identifier representing the specific lead.</value>
        [Key]
        [Column("lead_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific lead.
        /// </summary>
        /// <value>Unique identifier representing the specific lead.</value>
        [Column("lead_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier representing the specific lead on the client side.
        /// It is very useful to avoid duplicate leads being created due to connectivity issues.
        /// <br />
        /// <b>Important!</b> This Uid could be not unique between users, 
        /// but is assumed to be unique within leads of a single user.
        /// <br />
        /// It could be null as well.
        /// </summary>
        /// <value>Unique identifier representing the specific lead on the client side.</value>
        [Column("clientside_uid", TypeName = "UniqueIdentifier")]
        public Guid? ClientsideUid { get; set; }

        /// <summary>
        /// Date and time of last update for the specific lead on the client side.
        /// It is used to check whether the new lead update from client should be accepted or rejected.
        /// <br />
        /// It could be null as well.
        /// </summary>
        /// 
        /// <value>Date and time of last update for the specific lead on the client side.</value>
        [Column("clientside_updated_at")]
        public DateTime? ClientsideUpdatedAt { get; set; }

        /// <summary>
        /// Unique identifier of the user owning this lead.
        /// </summary>
        /// <value>Unique identifier of the user owning this lead.</value>
        [Column("user_uid")]
        public Guid UserUid { get; set; }

        [ForeignKey("UserUid")]
        public SubscriptionMember User { get; set; }

        /// <summary>
        /// Foreign key to the subscription whose user is owning this record.
        /// </summary>
        /// <value>Foreign key to the subscription whose user is owning this record.</value>
        [Column("subscription_id")]
        public long? SubscriptionId { get; set; }

        /// <summary>
        /// Actual subscription object for the user is owning this record.
        /// </summary>
        /// <value>Actual subscription object for the user is owning this record.</value>
        //[ForeignKey("SubscriptionId")]
        public SubscriptionRecord Subscription { get; set; }

        /// <summary>
        /// Unique identifier of the event the lead came from.
        /// </summary>
        /// <value>Unique identifier of the event the lead came from.</value>
        [Column("event_id")]
        public long? EventId { get; set; }

        /// <summary>
        /// Event the lead came from.
        /// </summary>
        /// <value>Event the lead came from.</value>
        [ForeignKey("EventId")]
        public EventRecord Event { get; set; }

        /// <summary>
        /// User's notes.
        /// </summary>
        /// <value>User's notes.</value>
        [Column("notes")]
        public string Notes { get; set; } = "";

        /// <summary>
        /// First name of the lead.
        /// </summary>
        /// <value>First name of the lead.</value>
        [Column("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the lead.
        /// </summary>
        /// <value>Last name of the lead.</value>
        [Column("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        /// <value>Company name</value>
        [Column("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Company URL
        /// </summary>
        /// <value>Company URL</value>
        [Column("company_url")]
        public string CompanyUrl { get; set; }

        /// <summary>
        /// Job title
        /// </summary>
        /// <value>Job title</value>
        [Column("job_title")]
        public string JobTitle { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        /// <value>Address</value>
        [Column("address")]
        public string Address { get; set; }

        /// <summary>
        /// Zip code
        /// </summary>
        /// <value>Zip code</value>
        [Column("zip_code")]
        public string ZipCode { get; set; }

        /// <summary>
        /// City name
        /// </summary>
        /// <value>City name</value>
        [Column("city")]
        public string City { get; set; }

        /// <summary>
        /// State/Region/Province name
        /// </summary>
        /// <value>State/Region/Province name</value>
        [Column("state")]
        public string State { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        /// <value>Country name</value>
        [Column("country")]
        public string Country { get; set; }

        /// <summary>
        /// URL for the person's photo
        /// </summary>
        /// <value>URL for the person's photo</value>
        [Column("photo_url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// URL for the thumbnail of the person's photo
        /// </summary>
        /// <value>URL for the thumbnail of the person's photo</value>
        [Column("photo_thumbnail_url")]
        public string PhotoThumbnailUrl { get; set; }

        /// <summary>
        /// URL for the business card's photo, back side
        /// </summary>
        /// <value>URL for the business card's photo, back side</value>
        [Column("business_card_back_url")]
        public string BusinessCardBackUrl { get; set; }

        /// <summary>
        /// URL for the thumbnail of the business card's photo, back side
        /// </summary>
        /// <value>URL for the thumbnail of the business card's photo, back side</value>
        [Column("business_card_back_thumbnail_url")]
        public string BusinessCardBackThumbnailUrl { get; set; }

        /// <summary>
        /// URL for the business card's photo, front side
        /// </summary>
        /// <value>URL for the business card's photo, front side</value>
        [Column("business_card_front_url")]
        public string BusinessCardFrontUrl { get; set; }

        /// <summary>
        /// URL for the thumbnail of the business card's photo, front side
        /// </summary>
        /// <value>URL for the thumbnail of the business card's photo, front side</value>
        [Column("business_card_front_thumbnail_url")]
        public string BusinessCardFrontThumbnailUrl { get; set; }

        /// <summary>
        /// String representation of FirstEntryLocation
        /// </summary>
        /// <value>String representation of FirstEntryLocation</value>
        [Column("location")]
        public string FirstEntryLocation { get; set; }

        /// <summary>
        /// Latitude of FirstEntryLocation
        /// </summary>
        /// <value>Latitude of FirstEntryLocation</value>
        [Column("location_latitude")]
        public double? FirstEntryLocationLatitude { get; set; }

        /// <summary>
        /// Longitude of FirstEntryLocation
        /// </summary>
        /// <value>Longitude of FirstEntryLocation</value>
        [Column("location_longitude")]
        public double? FirstEntryLocationLongitude { get; set; }

        /// <summary>
        /// Lead qualification
        /// </summary>
        /// <value>Lead qualification</value>
        [Column("qualification")]
        public LeadQualification Qualification { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// List of lead's phones
        /// </summary>
        /// <value>List of lead's phones</value>
        public List<LeadPhone> Phones { get; set; }

        /// <summary>
        /// List of lead's emails.
        /// </summary>
        /// <value>List of lead's emails.</value>
        public List<LeadEmail> Emails { get; set; }

        /// <summary>
        /// List of lead's answers to questions.
        /// </summary>
        /// <value>List of lead's answers to questions.</value>
        public List <LeadQuestionAnswer> QuestionAnswers { get; set; }

        /// <summary>
        /// List of lead's export statuses.
        /// </summary>
        /// <value>List of lead's export statuses.</value>
        public List<LeadExportStatus> ExportStatuses { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Lead {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  LeadUid: ").Append(Uid).Append("\n");
            sb.Append("  EventId: ").Append(EventId).Append("\n");
            sb.Append("  Notes: ").Append(Notes).Append("\n");
            sb.Append("  FirstName: ").Append(FirstName).Append("\n");
            sb.Append("  LastName: ").Append(LastName).Append("\n");
            sb.Append("  CompanyName: ").Append(CompanyName).Append("\n");
            sb.Append("  CompanyUrl: ").Append(CompanyUrl).Append("\n");
            sb.Append("  JobTitle: ").Append(JobTitle).Append("\n");
            sb.Append("  Address: ").Append(Address).Append("\n");
            sb.Append("  ZipCode: ").Append(ZipCode).Append("\n");
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  State: ").Append(State).Append("\n");
            sb.Append("  Country: ").Append(Country).Append("\n");
            sb.Append("  PhotoUrl: ").Append(PhotoUrl).Append("\n");
            sb.Append("  BusinessCardFrontUrl: ").Append(BusinessCardFrontUrl).Append("\n");
            sb.Append("  BusinessCardBackUrl: ").Append(BusinessCardBackUrl).Append("\n");
            sb.Append("  FirstEntryLocation: ").Append(FirstEntryLocation).Append("\n");
            sb.Append("  FirstEntryLocationLatitude: ").Append(FirstEntryLocationLatitude).Append("\n");
            sb.Append("  FirstEntryLocationLongitude: ").Append(FirstEntryLocationLongitude).Append("\n");
            sb.Append("  Phones: [\n").Append(Phones.Select(record => record.ToJson()).Aggregate((total, append) => total + ",\n" + append)).Append("]\n");
            sb.Append("  Emails: [\n").Append(Emails.Select(record => record.ToJson()).Aggregate((total, append) => total + ",\n" + append)).Append("]\n");
            sb.Append("  QuestionAnswers: [\n").Append(QuestionAnswers.Select(record => record.ToJson()).Aggregate((total, append) => total + ",\n" + append)).Append("]\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
