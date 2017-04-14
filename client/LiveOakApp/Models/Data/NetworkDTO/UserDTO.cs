using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class UserDTO
    {
        [DataMember(Name = "uid")]
        public Guid UID { get; set; } 

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "role")]
        public string RoleString { get; set; }

        public UserRole Role 
        {
            get
            {
                switch (RoleString) 
                {
                    case "anonymous": return UserRole.Anonymous;
                    case "seat_user": return UserRole.SeatUser;
                    case "tenant_admin": return UserRole.Admin;
                    case "super_admin": return UserRole.SuperAdmin;
                    default: return UserRole.Anonymous;
                }
            }
            set 
            {
                RoleString = value.ToString();
            }
        }

        public enum UserRole
        {
            Anonymous,
            SeatUser,
            Admin,
            SuperAdmin
        }
    }
}
