using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "expired_subscription_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "expired_subscription_notification")]
    public class SubscriptionExpiredNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }

        [XmlElement("subscription")]
        [DataMember(Name = "subscription")]
        public DataTypes.Subscription Subscription { get; set; }
    }
}