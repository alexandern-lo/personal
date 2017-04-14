using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    /// <summary>
    /// The model for storing user settings.
    /// </summary>
    [Table("crms")]
    public class CrmRecord
    {
        /// <summary>
        /// Bigint identifier representing the specific record.
        /// </summary>
        /// <value>Bigint identifier representing the specific record.</value>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// Local GUID indentifier for the corresponding record.
        /// </summary>
        /// <value>Local GUID indentifier for the corresponding record.</value>
        [Column("uid", TypeName = "UniqueIdentifier")]
        public Guid Uid { get; set; }

        /// <summary>
        /// Unique identifier of the user that .
        /// </summary>
        /// <value>Unique identifier of the user that signed up for this transaction.</value>
        [Column("user_uid")]
        public Guid UserUid { get; set; }

        [Column("type")]
        public CrmSystemAbbreviation CrmType { get; set; }

        /// <summary>
        /// User's name of this configuration.
        /// </summary>
        /// <value>User's name of this configuration.</value>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Access token for this CRM.
        /// </summary>
        /// <value>Access token for this CRM.</value>
        [Column("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token for this CRM.
        /// </summary>
        /// <value>Refresh token for this CRM.</value>
        [Column("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Field mappings in JSON format that describe how local fields will be mapped to CRM during export / synchronization.
        /// </summary>
        /// <value>Field mappings in JSON format that describe how local fields will be mapped to CRM during export / synchronization.</value>
        [Column("sync_fields", TypeName = "VARCHAR(MAX)")]
        public string SyncFields { get; set; }

        /// <summary>
        /// If CrmSystem is 'Dynamics365' then this field contains Dynamics 365 instance URL. 
        /// </summary>
        [Column("url", TypeName = "VARCHAR(2048)")]
        public string Url { get; set; }

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

        [Column("settings_id")]
        public long? SettingsId { get; set; }

        [ForeignKey("SettingsId")]
        public SettingsRecord Settings { get; set; }
    }
}