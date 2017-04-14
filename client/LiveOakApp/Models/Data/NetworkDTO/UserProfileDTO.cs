using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Contains general information on user record and his/her current subscription details.
    /// </summary>
    [DataContract]
    public class UserProfileDTO
    {
        public UserProfileDTO()
        {
        }

        /// <summary>
        /// Unique identifier representing the current user.
        /// </summary>
        /// <value>Unique identifier representing the current user.</value>
        [DataMember(Name = "user")]
        public UserDTO User { get; set; }

        /// <summary>
        /// Current subscription data for the current user.
        /// </summary>
        /// <value>Current subscription data for the current user.</value>
        [DataMember(Name = "subscription")]
        public UserSubscriptionDTO CurrentSubscription { get; set; }

        /// <summary>
        /// Terms that were accepted by the current user.
        /// </summary>
        /// <value>Terms that were accepted by the current user.</value>
        [DataMember(Name = "accepted_terms")]
        public TermsOfUseDTO AcceptedTerms { get; set; }

        /// <summary>
        /// All CRM configurations for this user.
        /// </summary>
        /// <value>All CRM configurations for this user.</value>
        [DataMember(Name = "default_crm")]
        public UserCrmConfigurationDTO CrmConfiguration { get; set; }
    }
}
