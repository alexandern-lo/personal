using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

using Avend.API.Infrastructure.SearchExtensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
    /// <summary>
    /// Enumeration for all possible subscrption services we support.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionServiceType
    {
        [EnumMember(Value = "recurly")] Recurly = 0,

        [EnumMember(Value = "subscription_bridge")] SubscriptionBridge = 1,

        [EnumMember(Value = "free_trial")] Free = 2,
    }

    /// <summary>
    /// Enumeration for all possible subscrption services we support.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionStatus
    {
        [EnumMember(Value = "pending")] Pending = 0,

        [EnumMember(Value = "active")] Active = 1,

        [EnumMember(Value = "expired")] Expired = 2,

        [EnumMember(Value = "cancelled")] Cancelled = 3,

        [EnumMember(Value = "suspended")] Suspended = 4,

        /// <summary>
        /// Special status that should be assigned to a subscription that is
        /// replaced by another one. It is important for proper statistical
        /// calculations.
        /// 
        /// Subscriptions with this status normally should not be returned 
        /// by any endpoints.
        /// </summary>
        [EnumMember(Value = "replaced")] Replaced = 5,
    }

    /// <summary>
    /// The model for storing user subscriptions in database.
    /// </summary>
    [Table("subscriptions")]
    [DefaultFilter("name")]
    public class SubscriptionRecord : BaseRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific user subscription record.
        /// </summary>
        /// <value>Bigint identifier representing the specific user subscription record.</value>
        [Key]
        [Column("id")]
        public override long Id { get; set; }

        /// <summary>
        /// Local GUID indentifier for the corresponding user subscription record.
        /// </summary>
        /// <value>Local GUID indentifier for the corresponding user subscription record.</value>
        [Column("uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier of the user that signed up for this subscription.
        /// </summary>
        /// <value>Unique identifier of the user that signed up for this subscription.</value>
        [Column("recurly_account_uid", TypeName = "UniqueIdentifier")]
        public Guid RecurlyAccountUid { get; set; }

        /// <summary>
        /// External UID string for identifying this user subscription record in external subscripions management system.
        /// </summary>
        /// <value>External UID string for identifying this user subscription record in external subscripions management system.</value>
        [Column("external_uid")]
        public string ExternalUid { get; set; }

        /// <summary>
        /// Type of subscription service used to set up this subscription.
        /// </summary>
        /// <value>Type of subscription service used to set up this subscription.</value>
        [Column("service_type")] //, TypeName = "ENUM('Recurly', 'SubscriptionBridge')")]
        public SubscriptionServiceType Service { get; set; }

        /// <summary>
        /// Current status of the subscription.
        /// </summary>
        /// <value>Current status of the subscription.</value>
        [Column("status")] //, TypeName = "ENUM('Pending', 'Active', 'Expired', 'Cancelled', 'Suspended')")]
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Additional subscription data received from the exernal subscription managing system.
        /// </summary>
        /// <value>Additional subscription data received from the exernal subscription managing system, serialized into JSON.</value>
        [Column("additional_data", TypeName = "VARCHAR(MAX)")]
        public string AdditionalData { get; set; }

        /// <summary>
        /// Subscription type.
        /// </summary>
        /// <value>Subscription type (individual / corporate is expected at this point).</value>
        [Column("type")]
        public string Type { get; set; }

        /// <summary>
        /// Maximum users count for subscription.
        /// </summary>
        /// <value>Maximum users count for subscription.</value>
        [Column("max_users_count")]
        public int MaximumUsersCount { get; set; }

        /// <summary>
        /// Active users count
        /// </summary>
        [Column("active_users_count")]
        public int ActiveUsersCount { get; set; }

        /// <summary>
        /// Date and time of expiry for this subscription.
        /// </summary>
        /// <value>Date and time of expiry for this subscription.</value>
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("name", TypeName = "VARCHAR(1024)")]
        public string Name { get; set; }
    }
}