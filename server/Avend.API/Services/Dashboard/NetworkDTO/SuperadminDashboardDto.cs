using System;
using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class SuperadminDashboardDto
    {
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "users")]
        public SuperadminUsersStatsDto UsersStats { get; set; }

        [DataMember(Name = "subscriptions")]
        public SuperadminSubscriptionsStatsDto SubscriptionStats { get; set; }

        [DataMember(Name = "leads")]
        public SuperadminLeadsStatsDto LeadsStats { get; set; }

        [DataMember(Name = "events")]
        public SuperadminEventsStatsDto EventsStats { get; set; }

        public SuperadminDashboardDto()
        {
            SubscriptionStats = new SuperadminSubscriptionsStatsDto();
            UsersStats = new SuperadminUsersStatsDto();

            LeadsStats = new SuperadminLeadsStatsDto();
            EventsStats = new SuperadminEventsStatsDto();
        }
    }
}
