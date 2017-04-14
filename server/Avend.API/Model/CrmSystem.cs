using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Avend.API.Model
{
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

    [Table("crm_systems")]
    public class CrmSystem: BaseRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific record.
        /// </summary>
        /// <value>Bigint identifier representing the specific record.</value>
        [Key]
        [Column("crm_system_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific record.
        /// </summary>
        /// <value>Unique identifier representing the specific record.</value>
        [Column("crm_system_uid")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// CRM abbreviation.
        /// </summary>
        /// <value>CRM abbreviation.</value>
        [Column("abbreviation")]
        public CrmSystemAbbreviation Abbreviation { get; set; }

        /// <summary>
        /// Crm system name.
        /// </summary>
        /// <value>Crm system name.</value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Params template for authorization request for this CRM.
        /// </summary>
        /// <value>Params template for authorization request for this CRM.</value>
        [Column("authorization_params", TypeName = "VARCHAR(MAX)")]
        public string AuthorizationParams { get; set; }

        /// <summary>
        /// URL where the API will get the access token.
        /// </summary>
        /// <value>URL where the API will get the access token.</value>
        [Column("token_request_url")]
        public string TokenRequestUrl { get; set; }

        /// <summary>
        /// Params template for token request request for this CRM.
        /// </summary>
        /// <value>Params template for token request for this CRM.</value>
        [Column("token_request_params", TypeName = "VARCHAR(MAX)")]
        public string TokenRequestParams { get; set; }

        /// <summary>
        /// Default field mappings stored in JSON format.
        /// </summary>
        /// <value>Default field mappings stored in JSON format.</value>
        [Column("default_field_mappings", TypeName = "VARCHAR(MAX)")]
        public string DefaultFieldMappings { get; set; }
    }
}