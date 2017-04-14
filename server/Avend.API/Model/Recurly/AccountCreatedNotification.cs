using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "new_account_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "new_account_notification")]
    public class AccountCreatedNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }
    }
}