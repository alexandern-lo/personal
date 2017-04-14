using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class FilterByEventsRequestDTO
    {
        [DataMember(Name = "limit")]
        public int? Limit  { get; set; }

        [DataMember(Name = "event_uids")]
        public List<Guid?> EventUids { get; set; }
    }
}