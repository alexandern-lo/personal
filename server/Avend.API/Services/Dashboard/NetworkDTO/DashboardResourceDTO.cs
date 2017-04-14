using System;
using System.Runtime.Serialization;
using Avend.API.Model;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract(Name = "dashboard_resource")]
    public class DashboardResourceDTO
    {
        [DataMember(Name = "resource_uid")]
        public Guid Uid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string MimeType { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "sent_count")]
        public int SentCount { get; set; }

        [DataMember(Name = "opened_count")]
        public int OpenedCount { get; set; }

        public static DashboardResourceDTO From(Resource obj)
        {
            var dto = new DashboardResourceDTO()
            {
                Uid = obj.Uid,

                Name = obj.Name,
                Url = obj.Url,

                MimeType = obj.MimeType,

                SentCount = obj.SentCount,
                OpenedCount = obj.OpenedCount,
            };

            return dto;
        }
    }
}