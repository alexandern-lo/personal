using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.Recurly.DataTypes;

namespace Avend.API.Model.Recurly
{
    [XmlRoot(ElementName = "closed_invoice_notification", Namespace = "", DataType = "string", IsNullable = true)]
    [DataContract(Name = "closed_invoice_notification")]
    public class InvoiceClosedNotification
    {
        [XmlElement("account")]
        [DataMember(Name = "account")]
        public Account Account { get; set; }

        [XmlElement("invoice")]
        [DataMember(Name = "invoice")]
        public Invoice Invoice { get; set; }
    }
}