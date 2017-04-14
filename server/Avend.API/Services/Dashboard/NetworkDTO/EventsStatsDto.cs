using System.ComponentModel;
using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class EventsStatsDto
    {
        [DataMember(Name = "total", EmitDefaultValue = true)]
        [DefaultValue(0)]
        public int? Total { get; set; }

        [DataMember(Name = "conference")]
        public int? Conference { get; set; }

        public override string ToString()
        {
            return $"{nameof(Total)}: {Total}, {nameof(Conference)}: {Conference}";
        }
    }
}