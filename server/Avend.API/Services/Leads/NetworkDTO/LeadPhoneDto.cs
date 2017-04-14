using System;
using System.Runtime.Serialization;
using System.Text;

using Avend.API.Model;

using Newtonsoft.Json;

namespace Avend.API.Services.Leads.NetworkDTO
{
    /// <summary>
    /// Network DTO for lead phone record
    /// </summary>
    [DataContract(Name = "lead_phone")]
    public class LeadPhoneDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeadPhoneDto" /> class.
        /// </summary>
        public LeadPhoneDto()
        {
        }

        /// <summary>
        /// Unique identifier of the LeadPhone object
        /// </summary>
        /// <value>Unique identifier of the LeadPhone object</value>
        [DataMember(Name = "lead_phone_uid")]
        public Guid? Uid { get; set; }

        /// <summary>
        /// Phone designation
        /// </summary>
        /// <value>Phone designation</value>
        [DataMember(Name = "designation")]
        public string Designation { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        /// <value>Phone</value>
        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("DTO LeadPhone {\n");
            sb.Append("  Designation: ").Append(Designation).Append("\n");
            sb.Append("  Phone: ").Append(Phone).Append("\n");
            
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

        public static LeadPhoneDto From(LeadPhone phoneObj)
        {
            var dto = new LeadPhoneDto()
            {
                Uid = phoneObj.Uid,

                Designation = phoneObj.Designation,

                Phone = phoneObj.Phone,
            };

            return dto;
        }
    }
}
