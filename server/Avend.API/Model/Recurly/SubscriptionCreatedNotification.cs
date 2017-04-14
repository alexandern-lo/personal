using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "new_subscription_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "new_subscription_notification")]
    public class SubscriptionCreatedNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }

        [XmlElement("subscription")]
        [DataMember(Name = "subscription")]
        public DataTypes.Subscription Subscription { get; set; }
    }
}