using System.ComponentModel;
using System.Runtime.Serialization;

namespace Avend.API.Services.Dashboard.NetworkDTO
{
    [DataContract]
    public class PaidVsTrialStatsDto
    {
        [DataMember(Name = "total", EmitDefaultValue = true)]
        [DefaultValue(0)]
        public int? Total { get; set; }

        [DataMember(Name = "trial")]
        public int? Trial { get; set; }

        [DataMember(Name = "paid")]
        public int? Paid { get; set; }

        public override string ToString()
        {
            return $"{nameof(Total)}: {Total}, {nameof(Trial)}: {Trial}, {nameof(Paid)}: {Paid}";
        }
    }
}