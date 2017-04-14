using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Avend.API.Model
{
    /// <summary>
    /// Lead email entry record.
    /// </summary>
    [Table("lead_emails")]
    public class LeadEmail : BaseRecord
    {
        /// <summary>
        /// Unique identifier representing the specific lead email.
        /// </summary>
        /// <value>Unique identifier representing the specific lead email.</value>
        [Key]
        [Column("lead_email_id")]
        public override long Id { get; set; }

        /// <summary>
        /// Foreign key to lead record this email belongs to.
        /// </summary>
        /// <value>Foreign key to lead record this email belongs to.</value>
        [Column("lead_id")]
        public long LeadId { get; set; }

        /// <summary>
        /// Lead record this email belongs to.
        /// </summary>
        /// <value>Parent Lead record this email belongs to.</value>
        [ForeignKey("LeadId")]
        [JsonProperty(IsReference = true, ReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        public LeadRecord LeadRecord { get; set; }

        /// <summary>
        /// Unique identifier representing the specific lead email.
        /// </summary>
        /// <value>Unique identifier representing the specific lead email.</value>
        [Column("lead_email_uid", TypeName = "UniqueIdentifier")]
        public override Guid Uid { get; set; }

        /// <summary>
        /// Email designation
        /// </summary>
        /// <value>Email designation</value>
        [Column("designation")]
        public string Designation { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// <value>Email</value>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class LeadEmail {\n");
            sb.Append("  UID: ").Append(Uid).Append("\n");
            sb.Append("  Designation: ").Append(Designation).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LeadEmail)obj);
        }

        /// <summary>
        /// Returns true if LeadEmail instances are equal
        /// </summary>
        /// <param name="other">Instance of LeadEmail to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(LeadEmail other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    this.Designation == other.Designation ||
                    this.Designation != null &&
                    this.Designation.Equals(other.Designation)
                ) &&
                (
                    this.Email == other.Email ||
                    this.Email != null &&
                    this.Email.Equals(other.Email)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)

                if (this.Designation != null)
                    hash = hash * 59 + this.Designation.GetHashCode();

                if (this.Email != null)
                    hash = hash * 59 + this.Email.GetHashCode();

                return hash;
            }
        }

        #region Operators

        public static bool operator ==(LeadEmail left, LeadEmail right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LeadEmail left, LeadEmail right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
