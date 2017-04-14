using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "successful_payment_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "successful_payment_notification")]
    public class PaymentSuccessfulNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }

        [XmlElement("transaction")]
        [DataMember(Name = "transaction")]
        public Transaction Transaction { get; set; }
    }
}