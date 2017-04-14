using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Avend.API.Services.Leads.NetworkDTO
{
    /// <summary>
    /// Network data object for passing over Uids for the leads to be exported.
    /// </summary>
    [DataContract]
    public class LeadsExportRequestDto
    {
        /// <summary>
        /// List of unique identifiers for the leads to be exported.
        /// </summary>
        /// 
        /// <value>List of unique identifiers for the leads to be exported. Nulls are allowed for fool-proof.</value>
        [DataMember(Name = "lead_uids")]
        public List<Guid?> Uids { get; set; }

        [DataMember(Name = "format")]
        public string Format { get; set; }
    }
}