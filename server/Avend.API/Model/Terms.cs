using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    [Table("terms")]
    public class Terms
    {
        public Terms()
        {
        }

        /// <summary>
        /// Bigint identifier representing the specific terms record.
        /// </summary>
        /// <value>Bigint identifier representing the specific terms record.</value>
        [Key]
        [Column("terms_id")]
        public long Id { get; set; }

        /// <summary>
        /// Unique identifier representing the specific terms record.
        /// </summary>
        /// <value>Unique identifier representing the specific terms record.</value>
        [Column("terms_uid")]
        public Guid Uid { get; set; }

        /// <summary>
        /// Terms text.
        /// </summary>
        /// <value>Terms text.</value>
        [Column("terms_text")]
        public string TermsText { get; set; }

        /// <summary>
        /// Terms release date.
        /// </summary>
        /// <value>Terms release date.</value>
        [Column("release_date")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Date and time of the record creation.
        /// </summary>
        /// <value>Date and time of the record creation.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time of last record update.
        /// </summary>
        /// <value>Date and time of last record update. Can be blank.</value>
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}