using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avend.API.Model
{
    [Table("terms_acceptances")]
    public class TermsAcceptance
    {
        public TermsAcceptance()
        {
        }

        /// <summary>
        /// Bigint identifier representing the specific terms acceptance record.
        /// </summary>
        /// <value>Bigint identifier representing the specific terms acceptance record.</value>
        [Key]
        [Column("terms_acceptance_id")]
        public long Id { get; set; }

        /// <summary>
        /// Foreign key for corresponding terms record.
        /// </summary>
        /// <value>Foreign key for corresponding terms record.</value>
        [Column("terms_id")]
        public long TermsId { get; set; }

        /// <summary>
        /// Unique identifier of the user that accepted the terms.
        /// </summary>
        /// <value>Unique identifier of the user that accepted the terms.</value>
        [Column("user_uid")]
        public Guid UserUid { get; set; }

        /// <summary>
        /// Acceptance date.
        /// </summary>
        /// <value>Acceptance date.</value>
        [Column("accepted_at")]
        public DateTime AcceptedAt { get; set; }

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