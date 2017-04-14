using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions;
using Newtonsoft.Json;

namespace Avend.API.Services.Leads.NetworkDTO
{
    /// <summary>
    /// Network data object for lead entity.
    /// </summary>
    [DataContract]
    public class LeadDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeadDto" /> class.
        /// </summary>
        public LeadDto()
        {
            Phones = new List<LeadPhoneDto>();
            Emails = new List<LeadEmailDto>();
            QuestionAnswers = new List<LeadQuestionAnswerDto>();
            ExportStatuses = new List<LeadExportStatusDto>();
        }

        /// <summary>
        /// Unique identifier representing the specific lead.
        /// </summary>
        /// <value>Unique identifier representing the specific lead.</value>
        [DataMember(Name = "lead_uid")]
        public Guid? Uid { get; set; }

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
        /// User who created this lead
        /// </summary>
        [DataMember(Name = "owner")]
        public SubscriptionMemberDto Owner { get; set; }

        /// <summary>
        /// User uid, used on updates to change owner
        /// </summary>
        [DataMember(Name = "user_uid")]
        public Guid? OwnerUid { get; set; }

        /// <summary>
        /// Owner tenant/subscription details
        /// </summary>
        [DataMember(Name = "tenant")]
        public SubscriptionDto Tenant { get; set; }

        [DataMember(Name = "event")]
        public EventDto Event { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid? EventUid { get; set; }

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
        /// URL for the thumbnail of the person's photo
        /// </summary>
        /// <value>URL for the thumbnail of the person's photo</value>
        [DataMember(Name = "photo_thumbnail_url")]
        public string PhotoThumbnailUrl { get; set; }

        /// <summary>
        /// URL for the business card's photo, front side
        /// </summary>
        /// <value>URL for the business card's photo, front side</value>
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
        /// Gets or Sets string representation of FirstEntryLocation
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
        public LeadQualification? Qualification { get; set; }

        /// <summary>
        /// Gets or Sets list of phones
        /// </summary>
        [DataMember(Name = "phones")]
        [JsonProperty]
        public List<LeadPhoneDto> Phones { get; set; }

        /// <summary>
        /// Gets or Sets list of emails
        /// </summary>
        [DataMember(Name = "emails")]
        [JsonProperty]
        public List<LeadEmailDto> Emails { get; set; }

        /// <summary>
        /// Gets or Sets list of Lead's question answers
        /// </summary>
        [DataMember(Name = "question_answers")]
        [JsonProperty]
        public List<LeadQuestionAnswerDto> QuestionAnswers { get; set; }

        /// <summary>
        /// Gets or Sets list of Lead's export statuses
        /// </summary>
        [DataMember(Name = "export_statuses")]
        [JsonProperty]
        public List<LeadExportStatusDto> ExportStatuses { get; set; }

        /// <summary>
        /// Date and time of the record's creation.
        /// </summary>
        /// <value>Date and time of the record's creation.</value>
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of the record's last update.
        /// </summary>
        /// <value>Date and time of the record's last update.</value>
        [DataMember(Name = "updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("DTO Lead {\n");
            sb.Append("  Uid: ").Append(Uid).Append("\n");
            sb.Append("  EventUid: ").Append(Event?.Uid).Append("\n");
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
            sb.Append("  Phones: ").Append(Phones).Append("\n");
            sb.Append("  Emails: ").Append(Emails).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static LeadDto From(LeadRecord leadObj)
        {
            return From(leadObj, (Guid?) null);
        }

        public static LeadDto From(LeadRecord leadObj, EventRecord eventObj)
        {
            return From(leadObj, eventObj?.Uid);
        }

        public static LeadDto From(LeadRecord leadObj, Guid? eventUid)
        {
            var dto = new LeadDto
            {
                Uid = leadObj.Uid,
                ClientsideUid = leadObj.ClientsideUid,
                ClientsideUpdatedAt = leadObj.ClientsideUpdatedAt,
                Tenant = leadObj.Subscription != null ? SubscriptionDto.From(leadObj.Subscription) : null,
                Owner = leadObj.User != null ? SubscriptionMemberDto.From(leadObj.User) : null,
                Event = leadObj.Event != null ? EventDto.From(leadObj.Event) : null,
                FirstName = leadObj.FirstName,
                LastName = leadObj.LastName,
                Qualification = leadObj.Qualification,
                Country = leadObj.Country,
                State = leadObj.State,
                City = leadObj.City,
                Address = leadObj.Address,
                ZipCode = leadObj.ZipCode,
                JobTitle = leadObj.JobTitle,
                CompanyName = leadObj.CompanyName,
                CompanyUrl = leadObj.CompanyUrl,
                FirstEntryLocation = leadObj.FirstEntryLocation,
                FirstEntryLocationLatitude = leadObj.FirstEntryLocationLatitude,
                FirstEntryLocationLongitude = leadObj.FirstEntryLocationLongitude,
                PhotoUrl = leadObj.PhotoUrl,
                PhotoThumbnailUrl = leadObj.PhotoThumbnailUrl,
                BusinessCardFrontUrl = leadObj.BusinessCardFrontUrl,
                BusinessCardFrontThumbnailUrl = leadObj.BusinessCardFrontThumbnailUrl,
                BusinessCardBackUrl = leadObj.BusinessCardBackUrl,
                BusinessCardBackThumbnailUrl = leadObj.BusinessCardBackThumbnailUrl,
                Notes = leadObj.Notes ?? "",
                Emails = leadObj.Emails.Select(LeadEmailDto.From).ToList(),
                Phones = leadObj.Phones.Select(LeadPhoneDto.From).ToList(),
                QuestionAnswers = leadObj.QuestionAnswers.Select(LeadQuestionAnswerDto.From).ToList(),
                ExportStatuses = leadObj.ExportStatuses.Select(LeadExportStatusDto.From).ToList(),
                CreatedAt = leadObj.CreatedAt,
                UpdatedAt = leadObj.UpdatedAt ?? leadObj.CreatedAt,
            };

            return dto;
        }

        public void UpdateLead(LeadRecord leadRecord)
        {
            if (ClientsideUid != null)
                leadRecord.ClientsideUid = ClientsideUid;

            if (ClientsideUpdatedAt != null)
                leadRecord.ClientsideUpdatedAt = ClientsideUpdatedAt;

            if (FirstName != null)
                leadRecord.FirstName = FirstName;
            if (LastName != null)
                leadRecord.LastName = LastName;
            if (Qualification.HasValue)
                leadRecord.Qualification = Qualification.Value;

            if (Country != null)
                leadRecord.Country = Country;
            if (State != null)
                leadRecord.State = State;
            if (City != null)
                leadRecord.City = City;
            if (Address != null)
                leadRecord.Address = Address;
            if (ZipCode != null)
                leadRecord.ZipCode = ZipCode;

            if (JobTitle != null)
                leadRecord.JobTitle = JobTitle;
            if (CompanyName != null)
                leadRecord.CompanyName = CompanyName;
            if (CompanyUrl != null)
                leadRecord.CompanyUrl = CompanyUrl;

            if (PhotoUrl != null)
                leadRecord.PhotoUrl = PhotoUrl;
            if (PhotoThumbnailUrl != null)
                leadRecord.PhotoThumbnailUrl = PhotoThumbnailUrl;

            if (BusinessCardFrontUrl != null)
                leadRecord.BusinessCardFrontUrl = BusinessCardFrontUrl;
            if (BusinessCardFrontThumbnailUrl != null)
                leadRecord.BusinessCardFrontThumbnailUrl = BusinessCardFrontThumbnailUrl;
            if (BusinessCardBackUrl != null)
                leadRecord.BusinessCardBackUrl = BusinessCardBackUrl;
            if (BusinessCardBackThumbnailUrl != null)
                leadRecord.BusinessCardBackThumbnailUrl = BusinessCardBackThumbnailUrl;

            if (Notes != null)
                leadRecord.Notes = Notes;
            if (leadRecord.Notes == null)
                leadRecord.Notes = "";

            if (FirstEntryLocation != null)
                leadRecord.FirstEntryLocation = FirstEntryLocation;
            if (FirstEntryLocationLatitude != null)
                leadRecord.FirstEntryLocationLatitude = FirstEntryLocationLatitude;
            if (FirstEntryLocationLongitude != null)
                leadRecord.FirstEntryLocationLongitude = FirstEntryLocationLongitude;

            leadRecord.Emails = Emails.Select(dto =>
            {
                var obj = new LeadEmail()
                {
                    Designation = dto.Designation,
                    Email = dto.Email,
                    Uid = dto.Uid ?? Guid.NewGuid()
                };

                return obj;
            }).ToList();

            leadRecord.Phones = Phones.Select(dto =>
            {
                var obj = new LeadPhone()
                {
                    Designation = dto.Designation,
                    Phone = dto.Phone,
                    Uid = dto.Uid ?? Guid.NewGuid()
                };

                return obj;
            }).ToList();

            leadRecord.QuestionAnswers = QuestionAnswers.Select(dto =>
            {
                var obj = new LeadQuestionAnswer()
                {
                    Uid = dto.Uid ?? Guid.NewGuid(),
                    EventQuestionUid = dto.EventQuestionUid,
                    EventAnswerUid = dto.EventAnswerUid,
                    QuestionText = dto.QuestionText,
                    AnswerText = dto.AnswerText,
                };

                return obj;
            }).ToList();
        }
    }
}