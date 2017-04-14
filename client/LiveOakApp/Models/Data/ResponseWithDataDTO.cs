using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data
{
    [DataContract]
    public class ResponseWithDataDTO<T>
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "data")]
        public T Data { get; set; }

        [DataMember(Name = "total_filtered_records")]
        public long? TotalFilteredRecords { get; set; }
    }
}
