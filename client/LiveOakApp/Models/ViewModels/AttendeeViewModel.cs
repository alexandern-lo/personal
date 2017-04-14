using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class AttendeeViewModel : DataContext
    {
        private AttendeeDTO AttendeeDTO { get; set; }
        public string UID { get { return AttendeeDTO.UID; } }
        public string FirstName { get { return AttendeeDTO.FirstName; } }
        public string LastName { get { return AttendeeDTO.LastName; } }
        public string Title { get { return AttendeeDTO.Title; } }
        public string Company { get { return AttendeeDTO.Company; } }
        public string Phone { get { return AttendeeDTO.Phone; } }
        public string Email { get { return AttendeeDTO.Email; } }
        public string AvatarUrl { get { return AttendeeDTO.AvatarUrl; } }
        public List<AttendeeCategoryValueDTO> CategoryValues { get { return AttendeeDTO.CategoryValues; } }

        public AttendeeViewModel(AttendeeDTO attendee)
        {
            AttendeeDTO = attendee;
        }

        public string AttendeeToJson()
        {
            return ServiceLocator.Instance.JsonService.Serialize(AttendeeDTO);
        }

        public static AttendeeViewModel JsonToAttendee(string attendee)
        {
            return new AttendeeViewModel(ServiceLocator.Instance.JsonService.Deserialize<AttendeeDTO>(attendee));
        }

        public string FullName
        {
            get
            {
                var parts = new List<string> { FirstName, LastName };
                return string.Join(" ", parts.Where(_ => !_.IsNullOrEmpty()));
            }
        }
    }
}
