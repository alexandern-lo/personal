using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using Avend.API.Infrastructure.SearchExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
    /// <summary>
    /// Resources are expected to be added by customers for private use and in events for public use.
    /// </summary>
    [Table("resources")]
    [DefaultFilter("Name")]
    public class Resource: BaseUserDependentRecord, IDeletable
    {
        /// <summary>
        /// Unique identifier representing the specific resource.
        /// </summary>
        /// <value>Unique identifier representing the specific resource.</value>
        [Key]
        [Column("resource_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific resource.
        /// </summary>
        /// <value>Unique identifier representing the specific resource.</value>
        [Column("resource_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier of the event the resource is bound to.
        /// </summary>
        /// <value>Unique identifier of the event the resource is bound to.</value>
        [Column("event_id")]
        public long? EventId { get; set; }

        [ForeignKey("EventId")]
        public EventRecord Event { get; set; }

        [Column("user_id")]
        public long? UserId { get; set; }

        [ForeignKey("UserId")]
        public SubscriptionMember User { get; set; }

        /// <summary>
        /// Name of the resource.
        /// </summary>
        /// <value>Name of the resource.</value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the resource.
        /// </summary>
        /// <value>Description of the resource.</value>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// URL to download resource.
        /// </summary>
        /// <value>URL to download resource</value>
        [Column("url")]
        public string Url { get; set; }

        /// <summary>
        /// MIME type of the resource as reported upon creation.
        /// </summary>
        /// <value>MIME type of the resource as reported upon creation</value>
        [Column("mime_type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Number of times this resource was sent.
        /// </summary>
        /// <value>Number of times this resource was sent</value>
        [Column("sent_count")]
        public int SentCount { get; set; }

        /// <summary>
        /// Number of times this resource was opened by recipient.
        /// </summary>
        /// <value>Number of times this resource was opened by recipient</value>
        [Column("opened_count")]
        public int OpenedCount { get; set; }

        /// <summary>
        /// Current status of the resource.
        /// </summary>
        /// <value>Current status of the resource.</value>
        [Column("status")]//, TypeName = "ENUM('Valid', 'Invalid')")]
        public ResourceStatus Status { get; set; }

        [Column("deleted")]
        public bool Deleted { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Resource {\n");
            sb.Append("  ResourceId: ").Append(Id).Append("\n");
            sb.Append("  ResourceUid: ").Append(Uid).Append("\n");
            sb.Append("  UserUid: ").Append(UserUid).Append("\n");
            sb.Append("  TenantUid: ").Append(TenantUid).Append("\n");
            sb.Append("  EventId: ").Append(EventId).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Url: ").Append(Url).Append("\n");
            sb.Append("  MimeType: ").Append(MimeType).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  SentCount: ").Append(SentCount).Append("\n");
            sb.Append("  OpenedCount: ").Append(OpenedCount).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
            sb.Append("}\n");

            return sb.ToString();
        }
    }

    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResourceStatus
    {
        [EnumMember(Value = "valid")]
        Valid,
        [EnumMember(Value = "invalid")]
        Invalid
    }
}
