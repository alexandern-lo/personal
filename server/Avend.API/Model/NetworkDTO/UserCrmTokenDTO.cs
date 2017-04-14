using System;
using System.Runtime.Serialization;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing terms to the user.
    /// </summary>
    [DataContract(Name = "crm_token")]
    public class UserCrmTokenDto
    {
        /// <summary>
        /// Uid of the user's CRM configuration record.
        /// </summary>
        /// <value>Uid of the user's CRM configuration record.</value>
        [DataMember(Name = "uid")]
        public Guid CrmConfigUid { get; set; }

        /// <summary>
        /// Token for the CRM - can be used for different tokens.
        /// </summary>
        /// <value>Token for the CRM - can be used for different tokens.</value>
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}