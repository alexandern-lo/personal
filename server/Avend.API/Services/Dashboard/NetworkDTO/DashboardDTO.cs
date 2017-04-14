using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract(Name = "dashboard")]
    public class DashboardDTO
    {
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "leads_statistics")]
        public LeadsStatisticsDTO LeadsStatistics { get; set; }

        [DataMember(Name = "events")]
        public List<DashboardEventDTO> Events { get; set; }

        [DataMember(Name = "resources")]
        public List<DashboardResourceDTO> Resources { get; set; }
    }

    [DataContract(Name = "leads_stats")]
    public class LeadsStatisticsDTO
    {
        [DataMember(Name = "alltime_count")]
        public int AllTimeCount { get; set; }

        [DataMember(Name = "alltime_goal")]
        public int AllTimeGoal { get; set; }

        [DataMember(Name = "last_period_count")]
        public int LastPeriodCount { get; set; }

        [DataMember(Name = "last_period_goal")]
        public int LastPeriodGoal { get; set; }

        [DataMember(Name = "this_year_expenses")]
        public MoneyDto ThisYearExpenses { get; set; }

        [DataMember(Name = "this_year_cpl")]
        public MoneyDto ThisYearCostPerLead { get; set; }
    }
}
