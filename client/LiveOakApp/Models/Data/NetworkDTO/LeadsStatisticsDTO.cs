using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "leads_stats")]
    public class LeadsStatisticsDTO
    {
        [DataMember(Name = "alltime_count")]
        public int AllTimeCount { get; set; }

        [DataMember(Name = "last_period_count")]
        public int LastPeriodCount { get; set; }

        [DataMember(Name = "last_period_goal")]
        public int LastPeriodGoal { get; set; }

        [DataMember(Name = "this_year_expenses")]
        public MoneyDTO ThisYearExpenses { get; set; }

        [DataMember(Name = "this_year_cpl")]
        public MoneyDTO ThisYearCostPerLead { get; set; }
    }
}
