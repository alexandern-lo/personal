using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    [DataContract]
    public class UserSubscriptionDTO
    {
        [DataMember(Name = "expired")]
        public bool Expired { get; set; }

        [DataMember(Name = "billing_period")]
        public string BillingPeriod { get; set; }

        [DataMember(Name = "max_users")]
        public int MaxUsers { get; set; }

        [DataMember(Name = "expires_at")]
        public DateTime ExpiresAt { get; set; }

    }
}
