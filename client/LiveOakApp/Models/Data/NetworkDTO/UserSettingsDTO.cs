using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing terms to the user.
    /// </summary>
    [DataContract(Name = "user_settings")]
    public class UserSettingsDTO
    {
        /// <summary>
        /// Foreign key to the default CRM selected by user.
        /// </summary>
        /// <value>Foreign key to the default CRM selected by user.</value>
        [DataMember(Name = "default_crm_configuration_uid")]
        public Guid? DefaultCRMConfigurationUID { get; set; }

        /// <summary>
        /// Abbreviation for the default CRM selected by user.
        /// </summary>
        /// <value>Abbreviation for the default CRM selected by user.</value>
        [DataMember(Name = "default_crm_abbrev")]
        public CrmSystemAbbreviation? DefaultCrmConfigurationAbbrev { get; set; }
    }

    /// <summary>
    /// Enumeration for all possible Crm System we support.
    /// </summary>
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrmSystemAbbreviation
    {
        [EnumMember(Value = "salesforce")]
        Salesforce = 0,

        [EnumMember(Value = "dynamics_365")]
        Dynamics365 = 1,
    }
}
