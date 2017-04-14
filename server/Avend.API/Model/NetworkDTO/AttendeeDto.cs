using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    public class AttendeeDto
    {
        public AttendeeDto()
        {
            CategoryValues = new List<AttendeeCategoryValueDto>();
        }

        [DataMember(Name = "attendee_uid")]
        public Guid? Uid { get; set; }

        [DataMember(Name = "event_uid")]
        public Guid? EventUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "company")]
        public string Company { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "avatar_url")]
        public string AvatarUrl { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip_code")]
        public string ZipCode { get; set; }

        [DataMember(Name = "categories")]
        public List<AttendeeCategoryValueDto> CategoryValues { get; set; }

        public static AttendeeDto From(AttendeeRecord obj, Guid? eventUid)
        {
            var dto = new AttendeeDto()
            {
                Uid = obj.Uid,
                EventUid = eventUid,

                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Title = obj.Title,
                AvatarUrl = obj.AvatarUrl,

                Company = obj.Company,

                Phone = obj.Phone,
                Email = obj.Email,

                Country = obj.Country,
                State = obj.State,
                City = obj.City,
                ZipCode = obj.ZipCode,

                CategoryValues = (obj.Values ?? new List<AttendeeCategoryValue>()).Select(AttendeeCategoryValueDto.From).ToList(),
            };

            return dto;
        }

        public void UpdateEventAttendee(AttendeeRecord attendeeObj)
        {
            if (FirstName != null)
                attendeeObj.FirstName = FirstName;

            if (LastName != null)
                attendeeObj.LastName = LastName;

            if (AvatarUrl != null)
                attendeeObj.AvatarUrl = AvatarUrl;

            if (Title != null)
                attendeeObj.Title = Title;

            if (Company != null)
                attendeeObj.Company = Company;

            if (Email != null)
                attendeeObj.Email = Email;

            if (Phone != null)
                attendeeObj.Phone = Phone;

            if (Country != null)
                attendeeObj.Country = Country;

            if (State != null)
                attendeeObj.State = State;

            if (City != null)
                attendeeObj.City = City;

            if (ZipCode != null)
                attendeeObj.ZipCode = ZipCode;
        }
    }
}
