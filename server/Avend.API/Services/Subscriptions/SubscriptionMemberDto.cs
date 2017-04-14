using System;
using System.Runtime.Serialization;
using Avend.API.Helpers;
using Avend.API.Model;
using Avend.API.Model.NetworkDTO;
using Avend.API.Services.Subscriptions.NetworkDTO;

namespace Avend.API.Services.Subscriptions
{
    [DataContract]
    public class SubscriptionMemberDto
    {
        public static SubscriptionMemberDto From(Model.SubscriptionMember member)
        {
            return new SubscriptionMemberDto
            {
                Role = RoleHelper.FromSubscriptionRole(member.Role),
                FirstName = member.FirstName,
                LastName = member.LastName,
                Email = member.Email,
                JobTitle = member.JobTitle,
                City = member.City,
                State = member.State,
                Status = member.Status,
                Uid = member.UserUid
            };
        }

        [DataMember(Name = "uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "status")]
        public SubscriptionMemberStatus Status { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "job_title")]
        public string JobTitle { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "role")]
        public UserRole Role { get; set; }
    }
}