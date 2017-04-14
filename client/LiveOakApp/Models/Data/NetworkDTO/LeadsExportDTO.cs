using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LiveOakApp.Models.Data.NetworkDTO
{
    /// <summary>
    /// Network data object for passing over Uids for the leads to be exported.
    /// </summary>
    [DataContract(Name = "leads_export")]
    public class LeadsExportDTO
    {
        /// <summary>
        /// Unique identifier representing the specific lead.
        /// </summary>
        /// <value>Unique identifier representing the specific lead.</value>
        [DataMember(Name = "lead_uids")]
        public List<string> Uids { get; set; }
    }
}
