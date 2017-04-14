using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Network data object for lead entity.
    /// </summary>
    [DataContract]
    public class LeadDTO
    {
        public LeadDTO()
        {
            Emails = new List<EmailDTO>();
            Phones = new List<PhoneDTO>();
            QuestionAnswers = new List<LeadQuestionAnswerDTO>();
            ExportStatuses = new List<LeadExportStatusDTO>();
        }

        /// <summary>
        /// Unique identifier representing the specific lead.
        /// </summary>
        /// <value>Unique identifier representing the specific lead.</value>
        [DataMember(Name = "lead_uid")]
        public string UID { get; set; }

        /// <summary>
        /// Unique identifier representing the specific lead on the client side.
        /// It is very useful to avoid duplicate leads being created due to connectivity issues.
        /// <br />
        /// <b>Important!</b> This Uid could be not unique between users, 
        /// but is assumed to be unique within leads of a single user.
        /// </summary>
        /// 
        /// <value>Unique identifier representing the specific lead on the client side.</value>
        [DataMember(Name = "client_uid")]
        public Guid? ClientsideUid { get; set; }

        /// <summary>
        /// Date and time of last update for the specific lead on the client side.
        /// It is used to check whether the new lead update from client should be accepted or rejected.
        /// <br />
        /// It could be null as well.
        /// </summary>
        /// 
        /// <value>Date and time of last update for the specific lead on the client side.</value>
        [DataMember(Name = "clientside_updated_at")]
        public DateTime? ClientsideUpdatedAt { get; set; }

        /// <summary>
        /// Unique identifier of the user the lead belongs to.
        /// </summary>
        /// <value>Unique identifier of the user the lead belongs to.</value>
        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

        /// <summary>
        /// Unique identifier of the tenant the lead belongs to.
        /// </summary>
        /// <value>Unique identifier of the tenant the lead belongs to.</value>
        [DataMember(Name = "tenant_uid")]
        public Guid? TenantUid { get; set; }

        /// <summary>
        /// Unique identifier of the event the lead came from.
        /// </summary>
        /// <value>Unique identifier of the event the lead came from.</value>
        [DataMember(Name = "event_uid")]
        public string EventUID { get; set; }

        [DataMember(Name = "event")]
        public EventDTO Event { get; set; }

        /// <summary>
        /// User's notes.
        /// </summary>
        /// <value>User's notes.</value>
        [DataMember(Name = "notes")]
        public string Notes { get; set; }

        /// <summary>
        /// First name of the lead.
        /// </summary>
        /// <value>First name of the lead.</value>
        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the lead.
        /// </summary>
        /// <value>Last name of the lead.</value>
        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        /// <value>Company name</value>
        [DataMember(Name = "company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Company URL
        /// </summary>
        /// <value>Company URL</value>
        [DataMember(Name = "company_url")]
        public string CompanyUrl { get; set; }

        /// <summary>
        /// Job title
        /// </summary>
        /// <value>Job title</value>
        [DataMember(Name = "job_title")]
        public string JobTitle { get; set; }

        /// <summary>
        /// Lead's Address
        /// </summary>
        /// <value>Lead's Address</value>
        [DataMember(Name = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Zip code
        /// </summary>
        /// <value>Zip code</value>
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
        public string Country { get; set; }

        /// <summary>
        /// URL for the person's photo
        /// </summary>
        /// <value>URL for the person's photo</value>
        [DataMember(Name = "photo_url")]
        public string PhotoUrl { get; set; }

        /// <summary>
        /// URL for the business card's photo
        /// </summary>
        /// <value>URL for the business card's photo</value>
        [DataMember(Name = "business_card_front_url")]
        public string BusinessCardFrontUrl { get; set; }

        /// <summary>
        /// URL for the thumbnail of the business card's photo, front side
        /// </summary>
        /// <value>URL for the thumbnail of the business card's photo, front side</value>
        [DataMember(Name = "business_card_front_thumbnail_url")]
        public string BusinessCardFrontThumbnailUrl { get; set; }

        /// <summary>
        /// URL for the business card's photo, back side
        /// </summary>
        /// <value>URL for the business card's photo, back side</value>
        [DataMember(Name = "business_card_back_url")]
        public string BusinessCardBackUrl { get; set; }

        /// <summary>
        /// URL for the thumbnail of business card's photo, back side
        /// </summary>
        /// <value>URL for the thumbnail of business card's photo, back side</value>
        [DataMember(Name = "business_card_back_thumbnail_url")]
        public string BusinessCardBackThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or Sets FirstEntryLocation
        /// </summary>
        [DataMember(Name = "location_string")]
        public string FirstEntryLocation { get; set; }

        /// <summary>
        /// Gets or Sets latitude of FirstEntryLocation
        /// </summary>
        [DataMember(Name = "location_latitude")]
        public double? FirstEntryLocationLatitude { get; set; }

        /// <summary>
        /// Gets or Sets longitude of FirstEntryLocation
        /// </summary>
        [DataMember(Name = "location_longitude")]
        public double? FirstEntryLocationLongitude { get; set; }

        /// <summary>
        /// Lead qualification
        /// </summary>
        /// <value>Lead qualification</value>
        [DataMember(Name = "qualification")]
        public string Classification { get; set; }

        /// <summary>
        /// Gets or Sets Phones
        /// </summary>
        [DataMember(Name = "phones")]
        public List<PhoneDTO> Phones { get; set; }

        /// <summary>
        /// Gets or Sets Emails
        /// </summary>
        [DataMember(Name = "emails")]
        public List<EmailDTO> Emails { get; set; }

        /// <summary>
        /// Gets or Sets list of Lead's question answers
        /// </summary>
        [DataMember(Name = "question_answers")]
        public List<LeadQuestionAnswerDTO> QuestionAnswers { get; set; }

        /// <summary>
        /// Gets or Sets list of Lead's export statuses
        /// </summary>
        [DataMember(Name = "export_statuses")]
        public List<LeadExportStatusDTO> ExportStatuses { get; set; }

        /// <summary>
        /// Date and time of the record's creation.
        /// </summary>
        /// <value>Date and time of the record's creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Date and time of the record's last update.
        /// </summary>
        /// <value>Date and time of the record's last update.</value>
        [DataMember(Name = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        #region Classification Enum

        public enum ClassificationType
        {
            Other,
            Hot,
            Warm,
            Cold
        }

        public ClassificationType ClassificationEnum
        {
            get
            {
                switch (Classification?.ToLower())
                {
                    case "hot": return ClassificationType.Hot;
                    case "warm": return ClassificationType.Warm;
                    case "cold": return ClassificationType.Cold;
                }
                return ClassificationType.Other;
            }
            set
            {
                
                Classification = value.ToString().ToLower();
            }
        }

        #endregion

        public void EnsureAllFieldsPresent()
        {
            UID = UID ?? "";
            EventUID = EventUID ?? "";
            Notes = Notes ?? "";
            FirstName = FirstName ?? "";
            LastName = LastName ?? "";
            CompanyName = CompanyName ?? "";
            CompanyUrl = CompanyUrl ?? "";
            JobTitle = JobTitle ?? "";
            Address = Address ?? "";
            ZipCode = ZipCode ?? "";
            City = City ?? "";
            State = State ?? "";
            Country = Country ?? "";
            PhotoUrl = PhotoUrl ?? "";
            BusinessCardFrontUrl = BusinessCardFrontUrl ?? "";
            BusinessCardBackUrl = BusinessCardBackUrl ?? "";
            FirstEntryLocation = FirstEntryLocation ?? "";
            FirstEntryLocationLatitude = FirstEntryLocationLatitude ?? null; // we never need to erase this
            FirstEntryLocationLongitude = FirstEntryLocationLongitude ?? null;
            Classification = Classification ?? "";
        }
    }
}
