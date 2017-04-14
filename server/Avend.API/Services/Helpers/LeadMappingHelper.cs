using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using Avend.API.Model;

using Newtonsoft.Json;

namespace Avend.API.Services.Helpers
{
    /// <summary>
    /// Static class to encapsulate all operations on lead properties.
    /// <br /><br />
    /// It also prepares those properties for fast access.
    /// </summary>
    public static class LeadMappingHelper
    {
        public static readonly IEnumerable<PropertyInfo> ExportableProperties = typeof(LeadRecord).GetProperties().Where(
                propItem =>
                {
                    var isExportableProp = propItem.CustomAttributes.Any(attrItem =>
                              attrItem.AttributeType == typeof(ColumnAttribute))
                              && propItem.CustomAttributes.All(attrItem => attrItem.AttributeType != typeof(KeyAttribute))
                              && propItem.CustomAttributes.All(attrItem => attrItem.AttributeType != typeof(ForeignKeyAttribute))
                              && propItem.Name != "EventId";

                    return isExportableProp;
                }
            );

        /// <summary>
        /// Returns the list of Lead properties that need to be exported.
        /// Relevant EventRecord properties and LeadsQuestionAnswer properties are added separately.
        /// </summary>
        /// <value>Dictionary of PropertyInfo objects indiced by field names for all lead properties that need to be exported to file</value>
        public static Dictionary<string, PropertyInfo> LeadFileExportPropertiesMap = ExportableProperties.ToDictionary(record => record.Name, record => record);

        /// <summary>
        /// Returns the list of Lead properties that could be potentially exported to CRM.
        /// The keys are DTO-styled (<b>snake_case</b>) so that it's easy to negotiate those for 
        /// mobile & web clients.
        /// </summary>
        /// 
        /// <value>Dictionary of PropertyInfo objects indiced by snake_cased field names for all lead properties that could be exported to CRM</value>
        public static Dictionary<string, PropertyInfo> LeadCrmExportPropertiesMap = ExportableProperties.ToDictionary(record => record.Name, record => record);

        public static Dictionary<string, PropertyInfo> LeadDtoPropertiesFieldMap = new Dictionary<string, PropertyInfo>()
            {
                {"lead_uid", LeadFileExportPropertiesMap[nameof(LeadRecord.Uid)]},
                {"user_uid", LeadFileExportPropertiesMap[nameof(LeadRecord.UserUid)]},
                {"notes", LeadFileExportPropertiesMap[nameof(LeadRecord.Notes)]},
                {"first_name", LeadFileExportPropertiesMap[nameof(LeadRecord.FirstName)]},
                {"last_name", LeadFileExportPropertiesMap[nameof(LeadRecord.LastName)]},
                {"company_name", LeadFileExportPropertiesMap[nameof(LeadRecord.CompanyName)]},
                {"company_url", LeadFileExportPropertiesMap[nameof(LeadRecord.CompanyUrl)]},
                {"job_title", LeadFileExportPropertiesMap[nameof(LeadRecord.JobTitle)]},
                {"zip_code", LeadFileExportPropertiesMap[nameof(LeadRecord.ZipCode)]},
                {"address", LeadFileExportPropertiesMap[nameof(LeadRecord.Address)]},
                {"city", LeadFileExportPropertiesMap[nameof(LeadRecord.City)]},
                {"state", LeadFileExportPropertiesMap[nameof(LeadRecord.State)]},
                {"country", LeadFileExportPropertiesMap[nameof(LeadRecord.Country)]},
                {"location_string", LeadFileExportPropertiesMap[nameof(LeadRecord.FirstEntryLocation)]},
                {"location_latitude", LeadFileExportPropertiesMap[nameof(LeadRecord.FirstEntryLocationLatitude)]},
                {"location_longitude", LeadFileExportPropertiesMap[nameof(LeadRecord.FirstEntryLocationLongitude)]},
                {"qualification", LeadFileExportPropertiesMap[nameof(LeadRecord.Qualification)]},
                {"phones", null},
                {"phone1", null},
                {"phone2", null},
                {"mobile_phone1", null},
                {"work_phone1", null},
                {"emails", null},
                {"email1", null},
                {"email2", null},
                {"exported_at", null},
                {"questions_and_answers", null},
                {"created_at", LeadFileExportPropertiesMap[nameof(LeadRecord.CreatedAt)]},
                {"updated_at", LeadFileExportPropertiesMap[nameof(LeadRecord.UpdatedAt)]},
            };

        /// <summary>
        /// Returns a property from Lead model by its name.
        /// <br /><br />
        /// It takes into account some virtual properties that aggregate data from dependent records.
        /// <br /><br />
        /// All properties are returned as strings.
        /// </summary>
        /// 
        /// <param name="leadRecord">Lead record to get property from</param>
        /// <param name="dtoField">Lead property name to get from the lead</param>
        /// 
        /// <returns>Stringified value of the property</returns>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        public static string GetDtoPropertyFromLead(LeadRecord leadRecord, string dtoField)
        {
            switch (dtoField)
            {
                case "questions_and_answers":
                    return ConstructLeadQuestionsAndAnswersFieldValue(leadRecord);

                case "phones":
                    return ConstructLeadPhonesFieldValue(leadRecord);

                case "phone1":
                    return leadRecord.Phones.Count > 0 ? leadRecord.Phones[0].Phone : "";

                case "phone2":
                    return leadRecord.Phones.Count > 1 ? leadRecord.Phones[0].Phone : "";

                case "mobile_phone1":
                {
                    var phone = (from p in leadRecord.Phones
                        where p.Designation == "mobile"
                        select p.Phone).FirstOrDefault();
                    return phone ?? "";
                }

                case "work_phone1":
                {
                    var phone = (from p in leadRecord.Phones
                                 where p.Designation == "work"
                                 select p.Phone).FirstOrDefault();
                        return phone ?? "";
                }

                case "emails":
                    return ConstructLeadEmailsFieldValue(leadRecord);

                case "email1":
                    return leadRecord.Emails.Count > 0 ? leadRecord.Emails[0].Email : "";

                case "email2":
                    return leadRecord.Emails.Count > 1 ? leadRecord.Emails[1].Email : "";
            }

            if (!LeadDtoPropertiesFieldMap.ContainsKey(dtoField))
                return null;

            return LeadDtoPropertiesFieldMap[dtoField]?.GetValue(leadRecord)?.ToString() ?? "";
        }

        private static string ConstructLeadEmailsFieldValue(LeadRecord leadRecord)
        {
            return leadRecord.Emails.Aggregate("", (left, right) => left + "\n" + right.Designation + ": " + right.Email);
        }

        private static string ConstructLeadPhonesFieldValue(LeadRecord leadRecord)
        {
            return leadRecord.Phones.Aggregate("", (left, right) => left + "\n" + right.Designation + ": " + right.Phone);
        }

        private static string ConstructLeadQuestionsAndAnswersFieldValue(LeadRecord leadRecord)
        {
            return leadRecord.QuestionAnswers.Aggregate("", (left, right) => left + "\n" + right.QuestionText + "? " + right.AnswerText);
        }

        /// <summary>
        /// Maps lead record to a dictionary based on the mapping provided by CrmRecord object.
        /// </summary>
        /// 
        /// <param name="leadRecord">Lead record to get data from</param>
        /// <param name="crmRecord">CRM configuration that provides mapping</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMappedLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            var preparedLead = new Dictionary<string, string>();

            if (crmRecord.SyncFields == null)
                return null;

            var dtoFieldMappings = JsonConvert.DeserializeObject<Dictionary<string, bool>>(crmRecord.SyncFields);

            if (dtoFieldMappings == null)
            {
                return null;
            }

            var avendToCrm = CrmDefaultsHelper.DefaultCrmMappings[crmRecord.CrmType];
            foreach (var mapping in dtoFieldMappings)
            {
                if (mapping.Value && avendToCrm.ContainsKey(mapping.Key))
                {
                    var crmField = avendToCrm[mapping.Key];
                    preparedLead[crmField] = GetDtoPropertyFromLead(leadRecord, mapping.Key);
                }
            }

            return preparedLead;
        }

        public static Dictionary<string, string> AdjustFieldsMapping(Dictionary<string, object> fieldMappings, CrmSystemAbbreviation abbreviation)
        {
            var adjustedMappings = new Dictionary<string, string>(CrmDefaultsHelper.DefaultCrmMappings[abbreviation]);
            var crmRemoteFieldsList = CrmDefaultsHelper.DefaultCrmMappings[abbreviation].Values.ToList();
            var keyCollection = new List<string>(adjustedMappings.Keys);
            foreach (var dtoField in keyCollection)
            {
                if (!fieldMappings.ContainsKey(dtoField))
                    adjustedMappings[dtoField] = null;
            }

            foreach (var mapping in fieldMappings)
            {
                var dtoField = mapping.Key;
                var crmField = mapping.Value;

                if (!LeadDtoPropertiesFieldMap.ContainsKey(dtoField))
                    continue;

                if (crmField == null)
                {
                    adjustedMappings[dtoField] = null;

                    continue;
                }

                var crmFieldBoolean = crmField as bool?;

                if (crmFieldBoolean.HasValue)
                {
                    if (!crmFieldBoolean.Value)
                        adjustedMappings[dtoField] = null;

                    continue;
                }

                var crmFieldString = crmField as string;

                if (crmRemoteFieldsList.Contains(crmFieldString))
                {
                    adjustedMappings[dtoField] = crmFieldString;

                    continue;
                }

                if (string.IsNullOrWhiteSpace(crmFieldString))
                    adjustedMappings[dtoField] = null;
            }

            return adjustedMappings;
        }
    }
}

