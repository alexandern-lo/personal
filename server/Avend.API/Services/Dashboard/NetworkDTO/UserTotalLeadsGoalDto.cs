using System;
using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class UserTotalLeadsGoalDto
    {
        [DataMember(Name = "user_uid")]
        public Guid? UserUid { get; set; }

        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [DataMember(Name = "leads_goal")]
        public int LeadsGoal { get; set; }

        [DataMember(Name = "leads_count")]
        public int LeadsCount { get; set; }
    }
}
