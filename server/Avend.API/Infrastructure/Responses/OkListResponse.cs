using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Encapsulates success response with accompanying list/array of records of the T type.
    /// Also provides total number of records, which is useful if in certain cases not all the records are returned.
    /// Expected to be initialized using an object initializer.
    /// </summary>
    [DataContract]
    public class OkListResponse<T> : OkResponse<List<T>>, IEquatable<OkListResponse<T>>
    {
        public OkListResponse()
        {
        }

        public OkListResponse(List<T> data, int total)
        {
            Data = data;
            TotalFilteredRecords = total;
        }

        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "total_filtered_records", Order = 5)]
        public long TotalFilteredRecords { get; set; }

        [DataMember(Name = "filters")]
        public Dictionary<string, object> Filters { get; set; }

        [DataMember(Name = "sort_field")]
        public string SortField { get; set; }

        [DataMember(Name = "sort_order")]
        public string SortOrder { get; set; }

        [DataMember(Name = "q")]
        public string Query { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class SuccessResponseDefault {\n");
            sb.Append("  Success: ").Append(Success).Append("\n");
            sb.Append("  Data: ").Append(Data != null ? Data.ToString() : "null").Append("\n");
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
            if (obj.GetType() != GetType())
                return false;

            return Equals((OkListResponse<T>) obj);
        }

        /// <summary>
        /// Returns true if SuccessResponseDefault instances are equal
        /// </summary>
        /// <param name="other">Instance of SuccessResponseDefault to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OkListResponse<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return (
                       this.Success == other.Success
                   ) && (
                       this.TotalFilteredRecords == other.TotalFilteredRecords
                   ) && (
                       this.Data != null && this.Data.Equals(other.Data)
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

                if (Data != null)
                    hash = hash * 59 + Data.GetHashCode();

                return hash;
            }
        }

        #endregion

        #region Operators

        public static bool operator ==(OkListResponse<T> left, OkListResponse<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OkListResponse<T> left, OkListResponse<T> right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}