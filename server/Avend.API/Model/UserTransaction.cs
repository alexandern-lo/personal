using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    /// <summary>
    /// Enumeration for all possible subscrption services we support.
    /// </summary>
    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 2,
        Suspended = 3,
    }

    /// <summary>
    /// The model for storing user transactions in database.
    /// </summary>
    [Table("user_transactions")]
    public class UserTransaction
    {
        /// <summary>
        /// Bigint identifier representing the specific user transaction record.
        /// </summary>
        /// <value>Bigint identifier representing the specific user transaction record.</value>
        [Key]
        [Column("user_transaction_id")]
        public long Id { get; set; }

        /// <summary>
        /// Local GUID indentifier for the corresponding user transaction record.
        /// </summary>
        /// <value>Local GUID indentifier for the corresponding user transaction record.</value>
        [Column("user_transaction_uid", TypeName = "UniqueIdentifier")]
        public Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier of the user that signed up for this transaction.
        /// </summary>
        /// <value>Unique identifier of the user that signed up for this transaction.</value>
        [Column("user_uid")]
        public Guid UserUid { get; set; }

        /// <summary>
        /// Foreign key to the subscription that this transaction belongs to.
        /// </summary>
        /// <value>Foreign key to the subscription that this transaction belongs to.</value>
        [Column("user_subscription_id")]
        public long SubscriptionId { get; set; }

        /// <summary>
        /// Subscription object that this transaction belongs to.
        /// </summary>
        /// <value>Subscription object that this transaction belongs to.</value>
        [ForeignKey("SubscriptionId")]
        public SubscriptionRecord Subscription { get; set; }

        /// <summary>
        /// External UID string for identifying this user transaction record in external subscripions management system.
        /// </summary>
        /// <value>External UID string for identifying this user transaction record in external subscripions management system.</value>
        [Column("external_uid")]
        public string ExternalUid { get; set; }

        /// <summary>
        /// Type of subscription service used to perform this transaction.
        /// </summary>
        /// <value>Type of subscription service used to perform this transaction.</value>
        [Column("service_type")] //, TypeName = "ENUM('Recurly', 'SubscriptionBridge')")]
        public SubscriptionServiceType Service { get; set; }

        /// <summary>
        /// Current status of the transaction.
        /// </summary>
        /// <value>Current status of the transaction.</value>
        [Column("status")] //, TypeName = "ENUM('Pending', 'Active', 'Expired', 'Cancelled', 'Suspended')")]
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// Additional transaction data received from the exernal subscription managing system.
        /// </summary>
        /// <value>Additional transaction data received from the exernal subscription managing system, serialized into JSON.</value>
        [Column("additional_data", TypeName = "VARCHAR(MAX)")]
        public string AdditionalData { get; set; }

        /// <summary>
        /// Date and time of record creation.
        /// </summary>
        /// <value>Date and time of record creation.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of latest record update.
        /// </summary>
        /// <value>Date and time of latest record update.</value>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}