using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class SuperadminSubscriptionsStatsDto
    {
        [DataMember(Name = "all_time")]
        public PaidVsTrialStatsDto AllTime { get; set; }

        [DataMember(Name = "last_period")]
        public PaidVsTrialStatsDto LastPeriod { get; set; }

        public SuperadminSubscriptionsStatsDto()
        {
            AllTime = new PaidVsTrialStatsDto();
            LastPeriod = new PaidVsTrialStatsDto();
        }
    }
}