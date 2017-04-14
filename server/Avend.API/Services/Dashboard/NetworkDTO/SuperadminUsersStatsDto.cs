using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class SuperadminUsersStatsDto
    {
        [DataMember(Name = "all_time")]
        public PaidVsTrialStatsDto AllTime { get; set; }

        [DataMember(Name = "yesterday")]
        public PaidVsTrialStatsDto Yesterday { get; set; }

        public SuperadminUsersStatsDto()
        {
            AllTime = new PaidVsTrialStatsDto();
            Yesterday = new PaidVsTrialStatsDto();
        }
    }
}