using System;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Network DTO for lead export status record
    /// </summary>
    [DataContract(Name = "lead_export_status")]
    public class LeadExportStatusDTO
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
    }
}
