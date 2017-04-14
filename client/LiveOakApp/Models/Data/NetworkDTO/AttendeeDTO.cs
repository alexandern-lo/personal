using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class AttendeeDTO
    {
        public AttendeeDTO()
        {
            CategoryValues = new List<AttendeeCategoryValueDTO>();
        }

        [DataMember(Name = "uid")]
        public string UID { get; set; }

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

        [DataMember(Name = "categories")]
        public List<AttendeeCategoryValueDTO> CategoryValues { get; set; }

    }
}
