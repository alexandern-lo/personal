using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    [Table("subscription_invites")]
    public class SubscriptionInvite
    {
        [Key]
        [Column("invite_id")]
        public long Id { get; set; }

        [Column("invite_uid")]
        public Guid Uid { get; set; }

        [Column("subscription_id")]
        public long SubscriptionId { get; set; }

        [ForeignKey("SubscriptionId")]
        public SubscriptionRecord Subscription { get; set; }

        [Column("subscription_member_id")]
        public long? SubscriptionMemberId { get; set; }

        [ForeignKey("SubscriptionMemberId")]
        public SubscriptionMember SubscriptionMember { get; set; }

        [Column("invite_code")]
        public string InviteCode { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("accepted")]
        public bool Accepted { get; set; }

        [Column("accepted_at")]
        public DateTime? AcceptedAt { get; set; }

        [Column("valid_till")]
        public DateTime ValidTill { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
