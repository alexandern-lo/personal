using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract(Name = "dashboard_resources")]
    public class DashboardResourceDTO
    {
        [DataMember(Name = "resource_uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "sent_count")]
        public int SentCount { get; set; }

        [DataMember(Name = "opened_count")]
        public int OpenedCount { get; set; }
    }
}

