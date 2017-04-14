using System;
using System.Runtime.Serialization;
using System.Text;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Encapsulates basic success response without any accompanying data.
    /// </summary>
    [DataContract]
    public class OkResponseEmpty : BaseResponse, IEquatable<OkResponseEmpty>
    {
        public OkResponseEmpty()
            : base(true)
        {
        }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SuccessResponseDefault {\n");
            sb.Append("  Success: ").Append(Success).Append("\n");
            
            sb.Append("}\n");
            return sb.ToString();
        }

        #region Equality

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType();
        }

        /// <summary>
        /// Returns true if SuccessResponseDefault instances are equal
        /// </summary>
        /// <param name="other">Instance of SuccessResponseDefault to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OkResponseEmpty other)
        {
            if (ReferenceEquals(null, other)) return false;

            if (ReferenceEquals(this, other)) return true;

            return  Success == other.Success || Success.Equals(other.Success);
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
                
                hash = hash * 59 + Success.GetHashCode();
                
                return hash;
            }
        }

        #endregion

        #region Operators

        public static bool operator ==(OkResponseEmpty left, OkResponseEmpty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OkResponseEmpty left, OkResponseEmpty right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
