using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "dashboard_event")]
    public class DashboardEventDTO
    {
        [DataMember(Name = "event_uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "website_url")]
        public string WebsiteUrl { get; set; }

        [DataMember(Name = "leads_goal")]
        public int LeadsGoal { get; set; }

        [DataMember(Name = "leads_count")]
        public int LeadsCount { get; set; }

        [DataMember(Name = "total_expenses")]
        public MoneyDTO TotalExpenses { get; set; }

    }
}
