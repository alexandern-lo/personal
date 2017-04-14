using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Avend.API.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Table("lead_export_statuses")]
    public class LeadExportStatus : BaseRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific record.
        /// </summary>
        /// <value>Bigint identifier representing the specific record.</value>
        [Column("lead_export_status_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific record.
        /// </summary>
        /// <value>Unique identifier representing the specific record.</value>
        [Column("lead_export_status_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier for this lead in external CRM.
        /// </summary>
        /// <value>Unique identifier for this lead in external CRM.</value>
        [Column("external_uid")]
        public string ExternalUid { get; set; }

        /// <summary>
        /// Foreign key for the lead this record describes.
        /// </summary>
        /// <value>Foreign key for the lead this record describes.</value>
        [Column("lead_id")]
        public long? LeadId { get; set; }

        /// <summary>
        /// Lead object this record describes.
        /// </summary>
        /// <value>Lead object this record describes.</value>
        [ForeignKey("LeadId")]
        public LeadRecord LeadRecord { get; set; }

        /// <summary>
        /// Foreign key for the user CRM configuration this record describes.
        /// </summary>
        /// <value>Foreign key for the user CRM configuration this record describes.</value>
        [Column("user_crm_configuration_uid", TypeName = "UniqueIdentifier")]
        public Guid? UserCrmConfigurationUid { get; set; }

        /// <summary>
        /// Date and time of export, if ever.
        /// </summary>
        /// <value>Returns date and time of last export.</value>
        [Column("exported_at")]
        public DateTime? ExportedAt { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LeadExportStatus {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Uid: ").Append(Uid).Append("\n");
            sb.Append("  LeadId: ").Append(LeadId).Append("\n");
            sb.Append("  ExportedAt: ").Append(ExportedAt).Append("\n");
            sb.Append("  CreatedAt: ").Append(CreatedAt).Append("\n");
            sb.Append("  UpdatedAt: ").Append(UpdatedAt).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
