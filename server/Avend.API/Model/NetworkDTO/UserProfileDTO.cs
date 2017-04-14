using System.Runtime.Serialization;
using Avend.API.Services.Subscriptions;
using Avend.API.Services.Subscriptions.NetworkDTO;
using Bogus.DataSets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model.NetworkDTO
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRole
    {
        [EnumMember(Value = "anonymous")] Anonymous,
        [EnumMember(Value = "seat_user")] SeatUser,
        [EnumMember(Value = "tenant_admin")] Admin,
        [EnumMember(Value = "super_admin")] SuperAdmin
    }

    /// <summary>
    /// Contains general information on user record and his/her current subscription details.
    /// </summary>
    [DataContract]
    public class UserProfileDto
    {
        [DataMember(Name = "user")]
        public SubscriptionMemberDto User { get; set; }

        [DataMember(Name = "tenant")]
        public TenantDto Tenant { get; set; }

        /// <summary>
        /// Current subscription data for the current user.
        /// </summary>
        /// <value>Current subscription data for the current user.</value>
        [DataMember(Name = "subscription")]
        public SubscriptionDto CurrentSubscription { get; set; }

        /// <summary>
        /// Terms that were accepted by the current user.
        /// </summary>
        /// <value>Terms that were accepted by the current user.</value>
        [DataMember(Name = "accepted_terms")]
        public TermsDTO AcceptedTerms { get; set; }

        [DataMember(Name = "default_crm")]
        public UserCrmDto DefaultCrm { get; set; }
    }
}