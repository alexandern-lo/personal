using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class SuperadminEventsStatsDto
    {
        [DataMember(Name = "all_time")]
        public EventsStatsDto AllTime { get; set; }

        public SuperadminEventsStatsDto()
        {
            AllTime = new EventsStatsDto();
        }
    }
}