using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Avend.API.Model.XmlNullable;

namespace Avend.API.Model.Recurly.DataTypes
{
    public class Subscription
    {
        [XmlElement("plan", IsNullable = true)]
        [DataMember(Name = "plan")]
        public SubscriptionPlan Plan { get; set; }

        [XmlElement("uuid", IsNullable = true)]
        [DataMember(Name = "uuid")]
        public Guid? Uid { get; set; }

        [XmlElement("state", DataType = "string", IsNullable = true)]
        [DataMember(Name = "state")]
        public string State { get; set; }

        [XmlElement("quantity", DataType = "int")]
        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [XmlElement("total_amount_in_cents", DataType = "int")]
        [DataMember(Name = "total_amount_in_cents")]
        public int AmountInCents { get; set; }

        [XmlElement("activated_at", IsNullable = true)]
        [DataMember(Name = "activated_at")]
        public XmlNullable<DateTime> ActivatedAt { get; set; }

        [XmlElement("canceled_at", IsNullable = true)]
        [DataMember(Name = "canceled_at")]
        public XmlNullable<DateTime> CanceledAt { get; set; }

        [XmlElement("expires_at", IsNullable = true)]
        [DataMember(Name = "expires_at")]
        public XmlNullable<DateTime> ExpiresAt { get; set; }

        [XmlElement("current_period_started_at", IsNullable = true)]
        [DataMember(Name = "current_period_started_at")]
        public XmlNullable<DateTime> CurrentPeriodStartsAt { get; set; }

        [XmlElement("current_period_ends_at", IsNullable = true)]
        [DataMember(Name = "current_period_ends_at")]
        public XmlNullable<DateTime> CurrentPeriodEndsAt { get; set; }

        [XmlElement("trial_started_at", IsNullable = true)]
        [DataMember(Name = "trial_started_at")]
        public XmlNullable<DateTime> TrialStartedAt { get; set; }

        [XmlElement("trial_ends_at", IsNullable = true)]
        [DataMember(Name = "trial_ends_at")]
        public XmlNullable<DateTime> TrialEndsAt { get; set; }
    }
}