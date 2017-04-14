using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class SuperadminLeadsStatsDto
    {
        [DataMember(Name = "all_time")]
        public PaidVsTrialStatsDto AllTime { get; set; }

        public SuperadminLeadsStatsDto()
        {
            AllTime = new PaidVsTrialStatsDto();
        }
    }
}