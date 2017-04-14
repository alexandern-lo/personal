using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class TermsOfUseDTO
    {
        [DataMember(Name = "terms_uid")]
        public string UID { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "release_date")]
        public DateTime ReleaseDate { get; set; }

        [DataMember(Name = "accepted_date")]
        public DateTime? AcceptedDate { get; set; }
    }
}
