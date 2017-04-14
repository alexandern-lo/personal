using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class EventDTO
    {
        public EventDTO()
        {
            Categories = new List<AttendeeCategoryDTO>();
            Questions = new List<EventQuestionDTO>();
        }

        // TODO: change to Guid? like on the server
        [DataMember(Name = "event_uid")]
        public string UID { get; set; }

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

        [DataMember(Name = "event_type")]
        public string Type { get; set; }

        [DataMember(Name = "website_url")]
        public string WebsiteUrl { get; set; }

        [DataMember(Name = "logo_url")]
        public string LogoUrl { get; set; }

        [DataMember(Name = "categories")]
        public List<AttendeeCategoryDTO> Categories { get; set; }

        [DataMember(Name = "questions")]
        public List<EventQuestionDTO> Questions { get; set; }

        [DataMember(Name = "recurring")]
        public bool IsReccuring { get; set; }
    }
}
