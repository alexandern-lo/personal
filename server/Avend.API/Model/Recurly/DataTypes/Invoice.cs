using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.XmlNullable;

namespace Avend.API.Model.Recurly.DataTypes
{
    [DataContract(Name = "invoice")]
    public class Invoice
    {
        [XmlElement("uuid", IsNullable = true)]
        [DataMember(Name = "uuid")]
        public string Uid { get; set; }

        [XmlElement("subscription_id", IsNullable = true)]
        [DataMember(Name = "subscription_id")]
        public Guid? SubscriptionUid { get; set; }

        [XmlElement("state", IsNullable = true)]
        [DataMember(Name = "state")]
        public string State { get; set; }

        [XmlElement("invoice_number_prefix", IsNullable = true)]
        [DataMember(Name = "invoice_number_prefix")]
        public string Prefix { get; set; }

        [XmlElement("invoice_number", DataType = "int")]
        [DataMember(Name = "invoice_number")]
        public int Number  { get; set; }

        [XmlElement("po_number", IsNullable = true)]
        [DataMember(Name = "po_number")]
        public string PoNumber { get; set; }

        [XmlElement("vat_number", IsNullable = true)]
        [DataMember(Name = "vat_number")]
        public string VatNumber { get; set; }

        [XmlElement("total_in_cents", DataType = "int")]
        [DataMember(Name = "total_in_cents")]
        public int AmountInCents { get; set; }

        [XmlElement("currency", IsNullable = true)]
        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [XmlElement("date", IsNullable = true)]
        [DataMember(Name = "date")]
        public XmlNullable<DateTime> CreatedAt { get; set; }

        [XmlElement("closed_at", IsNullable = true)]
        [DataMember(Name = "closed_at")]
        public XmlNullable<DateTime> ClosedAt { get; set; }

        [XmlElement("net_terms", DataType = "int")]
        [DataMember(Name = "net_terms")]
        public int NetTerms { get; set; }

        [XmlElement("collection_method", IsNullable = true)]
        [DataMember(Name = "collection_method")]
        public string CollectionMethod { get; set; }
    }
}