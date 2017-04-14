using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "billing_info_updated_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "billing_info_updated_notification")]
    public class AccountBillingUpdatedNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }
    }
}