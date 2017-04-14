using System;
using System.Runtime.Serialization;
using System.Text;

using Avend.API.Model;

using Newtonsoft.Json;

namespace Avend.API.Services.Leads.NetworkDTO
{
    /// <summary>
    /// Network DTO for lead export status record
    /// </summary>
    [DataContract(Name = "lead_export_status")]
    public class LeadExportStatusDto
    {
        /// <summary>
        /// Unique identifier of the LeadExportStatus object
        /// </summary>
        /// <value>Unique identifier of the LeadExportStatus object</value>
        [DataMember(Name = "lead_export_status_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Uid of the user CRM configuration record.
        /// </summary>
        /// <value>Uid of the user CRM configuration record.</value>
        [DataMember(Name = "user_crm_configuration_uid")]
        public Guid? UserCrmConfigurationUid { get; set; }

        /// <summary>
        /// Uid of this record in the external CRM system.
        /// </summary>
        /// <value>Uid of this record in the external CRM system.</value>
        [DataMember(Name = "external_uid")]
        public string ExternalUid { get; set; }

        /// <summary>
        /// Date and time of the last successful export event
        /// </summary>
        /// <value>Date and time of the last successful export event</value>
        [DataMember(Name = "exported_at")]
        public DateTime? ExportedAt { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("DTO LeadExportStatus {\n");
            sb.Append("  ExternalUid: ").Append(ExternalUid).Append("\n");
            sb.Append("  ExportedAt: ").Append(ExportedAt).Append("\n");
            sb.Append("}\n");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static LeadExportStatusDto From(LeadExportStatus exportStatusObj)
        {
            var dto = new LeadExportStatusDto()
            {
                Uid = exportStatusObj.Uid,

                UserCrmConfigurationUid = exportStatusObj.UserCrmConfigurationUid,

                ExternalUid = exportStatusObj.ExternalUid,

                ExportedAt = exportStatusObj.ExportedAt,
            };

            return dto;
        }
    }
}
