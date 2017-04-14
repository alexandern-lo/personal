using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class EmailDTO
    {
        public enum EmailType
        {
            Home,
            Work,
            Other
        }

        [DataMember(Name = "lead_email_uid")]
        public string UID { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "designation")]
        public string Designation { get; set; }

        public EmailType TypeEnum
        {
            get
            {
                switch (Designation?.ToLower())
                {
                    case "other": return EmailType.Other;
                    case "home": return EmailType.Home;
                    case "work": return EmailType.Work;
                }
                return EmailType.Other;
            }
            set
            {
                Designation = value.ToString();
            }
        }
    }
}
