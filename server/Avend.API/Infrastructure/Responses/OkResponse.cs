using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Avend.API.Infrastructure.SearchExtensions.Data;
using Avend.API.Model.NetworkDTO;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Encapsulates success response with accompanying data of the T type.
    /// Expected to be initialized using an object initializer.
    /// </summary>
    [DataContract]
    public class OkResponse<T> : BaseResponse, IEquatable<OkResponse<T>>
    {
        public OkResponse()
            : base(true)
        {
        }

        public OkResponse(T data)
            : base(true)
        {
            Data = data;
        }
       
        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [DataMember(Name = "data", Order = 10)]
        public T Data { get; set; }

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

            return Equals((OkResponse<T>)obj);
        }

        /// <summary>
        /// Returns true if SuccessResponseDefault instances are equal
        /// </summary>
        /// <param name="other">Instance of SuccessResponseDefault to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(OkResponse<T> other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Success == other.Success
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

        public static bool operator ==(OkResponse<T> left, OkResponse<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OkResponse<T> left, OkResponse<T> right)
        {
            return !Equals(left, right);
        }

        internal object WithData(UserCrmDto result)
        {
            throw new NotImplementedException();
        }

        #endregion Operators
    }

    public static class OkResponse
    {
        public static OkResponse<U> WithData<U>(U data)
        {
            return new OkResponse<U>(data);
        }

        public static OkListResponse<U> WithList<U>(IEnumerable<U> data, int total)
        {
            return new OkListResponse<U>(data.ToList(), total);
        }

        public static readonly OkResponseEmpty Empty = new OkResponseEmpty();

        public static OkListResponse<T> FromSearchResult<T>(SearchResult<T> searchResult)
        {
            return new OkListResponse<T>
            {
                Data = searchResult.Data.ToList(),
                TotalFilteredRecords = searchResult.Total,
                SortOrder = searchResult.QueryParams?.SortOrder,
                SortField = searchResult.QueryParams?.SortField,
                Query = searchResult.QueryParams?.Filter,
                Filters = searchResult.Filters
            };
        }
    }
}
