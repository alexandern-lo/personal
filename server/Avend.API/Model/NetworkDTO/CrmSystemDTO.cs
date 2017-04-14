using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Avend.API.Model.NetworkDTO
{
    /// <summary>
    /// Network DTO class for passing CRM system metadata to the user.
    /// </summary>
    [DataContract(Name = "crm_system")]
    public class CrmSystemDTO
    {
        /// <summary>
        /// Uid of the CRM system record.
        /// </summary>
        /// <value>Uid of the CRM system record.</value>
        [DataMember(Name = "crm_system_uid")]
        public Guid Uid { get; set; }

        /// <summary>
        /// CRM abbreviation.
        /// </summary>
        /// <value>CRM abbreviation.</value>
        [DataMember(Name = "abbreviation")]
        public CrmSystemAbbreviation Abbreviation { get; set; }

        /// <summary>
        /// Name of this CRM system.
        /// </summary>
        /// <value>Name of this CRM system.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Default field mappings as a dictionary.
        /// </summary>
        /// <value>Default field mappings as a dictionary.</value>
        [DataMember(Name = "default_field_mappings")]
        public Dictionary<string, string> DefaultFieldMappings { get; set; }

        public static CrmSystemDTO From(CrmSystem crmSystemObj, string authorizationUrl = null)
        {
            var dto = new CrmSystemDTO()
            {
                Uid = crmSystemObj.Uid,
                Abbreviation = crmSystemObj.Abbreviation,
                Name = crmSystemObj.Name,
                DefaultFieldMappings = JsonConvert.DeserializeObject(crmSystemObj.DefaultFieldMappings, typeof(Dictionary<string, string>)) as Dictionary<string, string>,
            };

            return  dto;
        }

        public void ApplyChangesToModel(CrmSystem crmSystem)
        {
            if (Name != null)
                crmSystem.Name = Name;
        }
    }
}