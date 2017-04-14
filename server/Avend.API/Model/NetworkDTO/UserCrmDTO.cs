using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing terms to the user.
    /// </summary>
    [DataContract(Name = "user_crm")]
    public class UserCrmDto
    {
        /// <summary>
        /// Uid of the CRM configuration record.
        /// </summary>
        /// <value>Uid of the CRM configuration record.</value>
        [DataMember(Name = "uid")]
        public Guid Uid { get; set; }

        /// <summary>
        /// User's name of this configuration.
        /// </summary>
        /// <value>User's name of this configuration.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Uid of the CRM system this configuration describes.
        /// </summary>
        /// <value>Uid of the CRM system this configuration describes.</value>
        [DataMember(Name = "type")]
        public CrmSystemAbbreviation? Type { get; set; }

        /// <summary>
        /// If CrmSystem is 'Dynamics365' then this field contains Dynamics 365 instance URL. 
        /// </summary>
        [DataMember(Name = "url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Indicates if the access token for this CRM is already acquired.
        /// </summary>
        /// <value>True if has access token for this CRM.</value>
        [DataMember(Name = "authorized")]
        public bool Authorized { get; set; }

        /// <summary>
        /// Field mappings as a dictionary.
        /// </summary>
        /// <value>Field mappings as a dictionary.</value>
        [DataMember(Name = "sync_fields")]
        public Dictionary<string, bool> SyncFields { get; set; }

        [DataMember(Name = "authorization_url")]
        public string AuthorizationUrl { get; set; }

        [DataMember(Name = "default")]
        public bool? Default { get; set; }
    }
}