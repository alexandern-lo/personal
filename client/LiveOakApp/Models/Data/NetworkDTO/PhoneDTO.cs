using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class PhoneDTO
    {
        public enum PhoneType
        {
            Home,
            Work,
            Mobile
        }

        [DataMember(Name = "lead_phone_uid")]
        public string UID { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "designation")]
        public string Designation { get; set; }

        public PhoneType TypeEnum
        {
            get
            {
                switch (Designation?.ToLower())
                {
                    case "home": return PhoneType.Home;
                    case "mobile": return PhoneType.Mobile;
                    case "work": return PhoneType.Work;
                }
                return PhoneType.Home;
            }
            set
            {
                Designation = value.ToString();
            }
        }
    }
}
