using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.XmlNullable;

namespace Avend.API.Model.Recurly.DataTypes
{
    [DataContract(Name = "transaction")]
    public class Transaction
    {
        [XmlElement("id")]
        [DataMember(Name = "id")]
        public string Uid { get; set; }

        [XmlElement("invoice_id")]
        [DataMember(Name = "invoice_id")]
        public Guid InvoiceId { get; set; }

        [XmlElement("invoice_number_prefix")]
        [DataMember(Name = "invoice_number_prefix")]
        public string InvoiceNumberPrefix { get; set; }

        [XmlElement("invoice_number")]
        [DataMember(Name = "invoice_number")]
        public long InvoiceNumber { get; set; }

        [XmlElement("subscription_id")]
        [DataMember(Name = "subscription_id")]
        public Guid? SubscriptionUid { get; set; }

        [XmlElement("action")]
        [DataMember(Name = "action")]
        public string Action { get; set; }

        [XmlElement("date", IsNullable = true)]
        [DataMember(Name = "date")]
        public XmlNullable<DateTime> CreatedAt { get; set; }

        [XmlElement("gateway")]
        [DataMember(Name = "gateway")]
        public string Gateway { get; set; }

        [XmlElement("payment_method")]
        [DataMember(Name = "payment_method")]
        public string PaymentMethod { get; set; }

        [XmlElement("amount_in_cents")]
        [DataMember(Name = "amount_in_cents")]
        public int AmountInCents { get; set; }

        [XmlElement("status")]
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [XmlElement("message")]
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [XmlElement("gateway_error_codes", IsNullable = true)]
        [DataMember(Name = "gateway_error_codes")]
        public string GatewayErrorCodes { get; set; }

        [XmlElement("failure_type", IsNullable = true)]
        [DataMember(Name = "failure_type")]
        public string FailureType { get; set; }

        [XmlElement("reference")]
        [DataMember(Name = "reference")]
        public string Reference { get; set; }

        [XmlElement("source")]
        [DataMember(Name = "source")]
        public string Source { get; set; }

        [XmlElement("cvv_result")]
        [DataMember(Name = "cvv_result")]
        public string CvvResult { get; set; }

        [XmlElement("avs_result")]
        [DataMember(Name = "avs_result")]
        public string AvsResult { get; set; }

        [XmlElement("avs_result_street")]
        [DataMember(Name = "avs_result_street")]
        public string AvsResultStreet { get; set; }

        [XmlElement("avs_result_postal")]
        [DataMember(Name = "avs_result_postal")]
        public string AvsResultPostal { get; set; }

        [XmlElement("billing_phone")]
        [DataMember(Name = "billing_phone")]
        public string BillingPhone { get; set; }

        [XmlElement("billing_postal")]
        [DataMember(Name = "billing_postal")]
        public string BillingPostal { get; set; }

        [XmlElement("billing_country")]
        [DataMember(Name = "billing_country")]
        public string BillingCountryCode { get; set; }

        [XmlElement("test")]
        [DataMember(Name = "test")]
        public bool? IsTest { get; set; }

        [XmlElement("voidable")]
        [DataMember(Name = "voidable")]
        public bool? IsVoidable { get; set; }

        [XmlElement("refundable")]
        [DataMember(Name = "refundable")]
        public bool? IsRefundable { get; set; }
    }
}