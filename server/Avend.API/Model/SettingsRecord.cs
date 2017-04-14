using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    /// <summary>
    /// The model for storing user settings.
    /// </summary>
    [Table("user_settings")]
    public class SettingsRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific record.
        /// </summary>
        /// <value>Bigint identifier representing the specific record.</value>
        [Key]
        [Column("user_settings_id")]
        public long Id { get; set; }

        /// <summary>
        /// Unique identifier of the user those settings belong to.
        /// </summary>
        /// <value>Unique identifier of the user those settings belong to.</value>
        [Column("user_uid")]
        public Guid UserUid { get; set; }

        /// <summary>
        /// Foreign key to the default CRM selected by user.
        /// </summary>
        /// <value>Foreign key to the default CRM selected by user.</value>
        [Column("default_user_crm_configuration_id")]
        public long? DefaultCrmId { get; set; }

        /// <summary>
        /// Default CRM system for the user.
        /// </summary>
        /// <value>Default CRM system for the user.</value>
        [ForeignKey("DefaultCrmId")]
        public CrmRecord DefaultCrm { get; set; }

        [Column("time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Date and time of record creation.
        /// </summary>
        /// <value>Date and time of record creation.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of latest record update.
        /// </summary>
        /// <value>Date and time of latest record update.</value>
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}