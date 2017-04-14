using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class DashboardDTO
    {
        public DashboardDTO()
        {
            Events = new List<DashboardEventDTO>();
            Resources = new List<DashboardResourceDTO>();
        }

        [DataMember(Name = "created_at")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "leads_statistics")]
        public LeadsStatisticsDTO LeadsStatistics { get; set; }

        [DataMember(Name = "events")]
        public List<DashboardEventDTO> Events { get; set; }

        [DataMember(Name = "resources")]
        public List<DashboardResourceDTO> Resources { get; set; }

    }
}
