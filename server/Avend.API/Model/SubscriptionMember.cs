using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Avend.API.Infrastructure.SearchExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionMemberStatus
    {
        [EnumMember(Value = "disabled")] Disabled = 0,
        [EnumMember(Value = "enabled")] Enabled = 1,
        [EnumMember(Value = "invited")] Invited = 2
    }

    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SubscriptionMemberRole
    {
        [EnumMember(Value = "user")] User = 0,
        [EnumMember(Value = "admin")] Admin = 1,
        [EnumMember(Value = "super_admin")] SuperAdmin = 2
    }

    [Table("subscription_members")]
    [DefaultFilter(new[] {"FirstName", "LastName"})]
    public class SubscriptionMember : BaseRecord
    {
        [Key]
        [Column("subscription_member_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific record.
        /// </summary>
        /// <value>Unique identifier representing the specific record.</value>
        [NotMapped]
        public override Guid Uid {
            get
            {
                return UserUid;
            }
            set
            {
                UserUid = value;
            }
        }

        [Column("user_uid")]
        public Guid UserUid { get; set; }

        [Column("status")]
        public SubscriptionMemberStatus Status { get; set; }

        [Column("subscription_id")]
        public long? SubscriptionId { get; set; }

        [ForeignKey("SubscriptionId")]
        public SubscriptionRecord Subscription { get; set; }

        [Column("role")]
        public SubscriptionMemberRole Role { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("job_title")]
        public string JobTitle { get; set; }

        [Column("city")]
        public string City { get; set; }

        [Column("state")]
        public string State { get; set; }

        public virtual ICollection<LeadRecord> Leads { get; set; }
    }
}