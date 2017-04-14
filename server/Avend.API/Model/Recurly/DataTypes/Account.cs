using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Avend.API.Model.Recurly.DataTypes
{
    [DataContract(Name = "account")]
    public class Account
    {
        [XmlElement("account_code", DataType = "string", IsNullable = true)]
        [DataMember(Name = "account_code")]
        public string AccountCode { get; set; }

        [XmlElement("username")]
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [XmlElement("email")]
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [XmlElement("first_name")]
        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        [XmlElement("last_name")]
        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        [XmlElement("company_name")]
        [DataMember(Name = "company_name")]
        public string CompanyName { get; set; }

        [XmlElement("phone")]
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
    }
}