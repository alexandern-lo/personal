using System;
using System.Runtime.Serialization;
using System.Text;

using Avend.API.Model;

using Newtonsoft.Json;

namespace Avend.API.Services.Leads.NetworkDTO
{
    /// <summary>
    /// Network DTO for lead email record
    /// </summary>
    [DataContract(Name = "lead_email")]
    public class LeadEmailDto
    {
        /// <summary>
        /// Unique identifier of the LeadEmail object
        /// </summary>
        /// <value>Unique identifier of the LeadEmail object</value>
        [DataMember(Name = "lead_email_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Email designation
        /// </summary>
        /// <value>Email designation</value>
        [DataMember(Name = "designation")]
        public string Designation { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        /// <value>Email</value>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("DTO LeadEmail {\n");
            sb.Append("  Designation: ").Append(Designation).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            
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

        public static LeadEmailDto From(LeadEmail emailObj)
        {
            var dto = new LeadEmailDto()
            {
                Uid = emailObj.Uid,

                Designation = emailObj.Designation,

                Email = emailObj.Email,
            };

            return dto;
        }
    }
}
