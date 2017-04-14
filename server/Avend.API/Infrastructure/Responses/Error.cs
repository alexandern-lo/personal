using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Avend.API.Infrastructure.Responses
{
    /// <summary>
    /// Contains data on a single error encountered while processing API request.
    /// Has Code, Message and Fields properties.
    /// </summary>
    [DataContract]
    public class Error :  IEquatable<Error>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error" /> class.
        /// </summary>
        /// <param name="code">Code (required) (default to &quot;unknown_error&quot;)</param>
        /// <param name="message">Message (required)</param>
        /// <param name="fields">Affected fields of the request</param>
        public Error(string code = null, string message = null, string[] fields = null) 
        {
            // "Code" is required (not null)
            if (code == null)
            {
                throw new InvalidDataException("Code is a required property for Error and cannot be null");
            }

            // "Message" is required (not null)
            if (message == null)
            {
                throw new InvalidDataException("Message is a required property for Error and cannot be null");
            }

            Code = code;

            Message = message;
            
            Fields = fields != null ? new List<string>(fields) : null;
        }

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Gets or Sets Code
        /// </summary>
        [DataMember(Name = "code")]
        public string Code { get; set; }
        
        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; }
        
        /// <summary>
        /// Gets or Sets Fields
        /// </summary>
        [DataMember(Name = "fields")]
        public List<string> Fields { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Error {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  Fields: ").Append(Fields).Append("\n");
            
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
            return Equals((Error)obj);
        }

        /// <summary>
        /// Returns true if Error instances are equal
        /// </summary>
        /// <param name="other">Instance of Error to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Error other)
        {

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return 
                (
                    this.Code == other.Code ||
                    this.Code != null &&
                    this.Code.Equals(other.Code)
                ) && 
                (
                    this.Message == other.Message ||
                    this.Message != null &&
                    this.Message.Equals(other.Message)
                ) && 
                (
                    this.Fields == other.Fields ||
                    this.Fields != null &&
                    this.Fields.SequenceEqual(other.Fields)
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
                
                    if (this.Code != null)
                    hash = hash * 59 + this.Code.GetHashCode();
                
                    if (this.Message != null)
                    hash = hash * 59 + this.Message.GetHashCode();
                
                    if (this.Fields != null)
                    hash = hash * 59 + this.Fields.GetHashCode();
                
                return hash;
            }
        }

        #region Operators

        public static bool operator ==(Error left, Error right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Error left, Error right)
        {
            return !Equals(left, right);
        }

        #endregion Operators

    }
}
