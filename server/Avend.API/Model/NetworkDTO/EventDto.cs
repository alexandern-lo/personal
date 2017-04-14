using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Avend.API.Services.Subscriptions;
using Newtonsoft.Json;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Data Transfer object for events.
    /// </summary>
    [DataContract(Name = "event")]
    public class EventDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDto" /> class.
        /// </summary>
        public EventDto()
        {
            AttendeeCategories = new List<AttendeeCategoryDto>();
        }
        
        /// <summary>
        /// Unique identifier representing the specific event.
        /// </summary>
        /// <value>Unique identifier representing the specific event.</value>
        [DataMember(Name = "event_uid")]
        public Guid? Uid { get; set; }
        /// <summary>
        /// Event type. Can only be conference or personal at the moment.
        /// </summary>
        /// <value>Event type. Can only be conference or personal at the moment.</value>
        [DataMember(Name = "event_type")]
        public string Type { get; set; }

        /// <summary>
        /// URL for event logo image.
        /// </summary>
        /// <value>URL for event logo image.</value>
        [DataMember(Name = "logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// Event website URL.
        /// </summary>
        /// <value>Event website URL.</value>
        [DataMember(Name = "website_url")]
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        /// <value>Event name</value>
        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        /// <summary>
        /// Event venue name
        /// </summary>
        /// <value>Event venue name</value>
        [DataMember(Name = "venue_name", EmitDefaultValue = true)]
        public string VenueName { get; set; }

        /// <summary>
        /// Start date, UTC
        /// </summary>
        /// <value>Start date, UTC</value>
        [DataMember(Name = "start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date, UTC
        /// </summary>
        /// <value>End date, UTC</value>
        [DataMember(Name = "end_date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        /// <value>Address</value>
        [DataMember(Name = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Zip/Postal Code
        /// </summary>
        /// <value>Zip/Postal Code</value>
        [DataMember(Name = "zip_code")]
        public string ZipCode { get; set; }

        /// <summary>
        /// City name
        /// </summary>
        /// <value>City name</value>
        [DataMember(Name = "city")]
        public string City { get; set; }

        /// <summary>
        /// State/Region/Province name
        /// </summary>
        /// <value>State/Region/Province name</value>
        [DataMember(Name = "state")]
        public string State { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        /// <value>Country name</value>
        [DataMember(Name = "country")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        /// <value>Industry</value>
        [DataMember(Name = "industry")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Industry { get; set; }

        /// <summary>
        /// Categories with possible options
        /// </summary>
        /// <value>Categories with possible options</value>
        [DataMember(Name = "categories")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AttendeeCategoryDto> AttendeeCategories { get; set; }

        /// <summary>
        /// Questions with answers
        /// </summary>
        /// <value>Questions with answers</value>
        [DataMember(Name = "questions")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EventQuestionDto> Questions { get; set; }

        [DataMember(Name = "description")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [DataMember(Name = "owner")]
        public SubscriptionMemberDto Owner { get; set; }

        [DataMember(Name = "tenant")]
        public string Tenant { get; set; }

        [DataMember(Name = "recurring")]
        public bool Recurring { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EventRecord {\n");
            sb.Append("  Uid: ").Append(Uid).Append("\n");
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
            sb.Append("  Categories: ").Append(AttendeeCategories).Append("\n");
            sb.Append("  Questions: ").Append(Questions).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }

        public static EventDto From(EventRecord eventObj)
        {
            var dto = new EventDto()
            {
                Uid = eventObj.Uid,
                Type = eventObj.Type,

                LogoUrl = eventObj.LogoUrl,
                WebsiteUrl = eventObj.WebsiteUrl,

                Name = eventObj.Name,
                VenueName = eventObj.VenueName,

                Country = eventObj.Country,
                State = eventObj.State,
                City = eventObj.City,
                ZipCode = eventObj.ZipCode,
                Address = eventObj.Address,

                Industry = eventObj.Industry,

                StartDate = eventObj.StartDate,
                EndDate = eventObj.EndDate,

                Tenant = eventObj.Subscription?.Name,
                Owner = eventObj.Owner != null ? SubscriptionMemberDto.From(eventObj.Owner) : null,
                Recurring = eventObj.Recurring,

                AttendeeCategories = eventObj.AttendeeCategories.Select(record => AttendeeCategoryDto.From(record, eventObj.Uid)).ToList(),
                Questions = eventObj.Questions.Select(record => EventQuestionDto.From(record, eventObj.Uid)).ToList(),
            };

            return dto;
        }
    }
}
