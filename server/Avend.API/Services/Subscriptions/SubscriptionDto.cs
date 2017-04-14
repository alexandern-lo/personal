using System;
using System.Runtime.Serialization;
using Avend.API.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Services.Subscriptions
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionBillingPeriod
    {
        [EnumMember(Value = "monthly")]
        Monthly = 0,
        [EnumMember(Value = "quarterly")]
        Quarterly = 1,
        [EnumMember(Value = "yearly")]
        Yearly = 2,
    }

    /// <summary>
    /// Contains details on user's current subscription.
    /// </summary>
    [DataContract]
    public class SubscriptionDto
    {
        /// <summary>
        /// Uid of the subscription in system database.
        /// </summary>
        /// <value>Uid of the subscription in system database.</value>
        [DataMember(Name = "subscription_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Status of the subscription for the current user.
        /// </summary>
        /// <value>Status of the subscription for the current user.</value>
        [DataMember(Name = "status")]
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Service of the subscription for the current user.
        /// </summary>
        /// <value>Service of the subscription for the current user.</value>
        [DataMember(Name = "service")]
        public SubscriptionServiceType Service { get; set; }

        /// <summary>
        /// Billing period of the subscription for the current user.
        /// </summary>
        /// <value>Billing period of the subscription for the current user.</value>
        [DataMember(Name = "billing_period")]
        public SubscriptionBillingPeriod BillingPeriod { get; set; }

        /// <summary>
        /// Maximum number of users allowed within the subscription for the current user.
        /// </summary>
        /// <value>Maximum number of users allowed within the subscription for the current user.</value>
        [DataMember(Name = "max_users")]
        public int MaxUsers { get; set; }

        /// <summary>
        /// Date and time of subscription's expiry for the current user.
        /// </summary>
        /// <value>Date and time of subscription's expiry for the current user.</value>
        [DataMember(Name = "expires_at")]
        public DateTime ExpiresAt { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Constructs proper UserSubscriptionDTO object based on Model object passed over.
        /// </summary>
        /// <param name="obj">Object to construct DTO from</param>
        /// <returns>Properly populated UserSubscriptionDTO object</returns>
        public static SubscriptionDto From(SubscriptionRecord obj)
        {
            var dto = new SubscriptionDto()
            {
                Uid = obj.Uid,
                MaxUsers = obj.MaximumUsersCount,
                BillingPeriod = SubscriptionBillingPeriod.Monthly,
                Status = (obj.Status != SubscriptionStatus.Active || obj.ExpiresAt >= DateTime.UtcNow) ? obj.Status : SubscriptionStatus.Expired,
                Service = obj.Service,
                ExpiresAt = obj.ExpiresAt,
                Name = obj.Name
            };

            return dto;
        }

        
    }
}