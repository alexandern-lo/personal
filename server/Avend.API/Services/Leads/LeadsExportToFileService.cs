using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services.Helpers;
using Avend.API.Services.Leads.NetworkDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using Qoden.Validation;

namespace Avend.API.Services.Leads
{
    /// <summary>
    /// Implements functions required for Leads export to both file and CRMs.
    /// <list type="bullet">
    /// <item>Export leads to the TSV/Excel file</item>
    /// <item>Get UID of default CRM configuration for user</item>
    /// <item>Get CRM configuration for user by its UID</item>
    /// <item>Construct URL for CRM authorization request</item>
    /// <item>Connect to CRM</item>
    /// </list>
    /// </summary>
    public class LeadsExportToFileService
    {
        public static Dictionary<string, string> ExtensionsMapping = new Dictionary<string, string>
        {
            {"excel", "xls"},
            {"csv", "csv"},
        };

        public static Dictionary<string, string> MimeMapping = new Dictionary<string, string>
        {
            {"excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"csv", "text/csv"},
        };

        private string FieldsSeparator { get; } = ",";
        private string FieldsScreener { get; } = "\"";

        public UserContext UserContext { get; }
        public DbContextOptions<AvendDbContext> DbOptions { get; }

        private ILogger Logger { get; }

        /// <summary>
        /// Default constructor.
        /// 
        /// Instantiates logger using AvendLog.
        /// </summary>
        public LeadsExportToFileService(
            UserContext userContext,
            DbContextOptions<AvendDbContext> dbOptions
        )
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();

            UserContext = userContext;

            DbOptions = dbOptions;

            Logger = AvendLog.CreateLogger<LeadsExportToFileService>();
        }

        /// <summary>
        /// Auxiliary function to prepare exported field.
        /// </summary>
        /// 
        /// <param name="val">Value to write</param>
        /// <param name="fieldsSeparator">Field separator that should be screened</param>
        /// <param name="fieldsScreener">Screener character to use</param>
        /// 
        /// <returns>Prepares the value string by screening the field separator characters in it</returns>
        private static string GetExportFileFormattedField(string val, string fieldsSeparator, string fieldsScreener)
        {
            var preparedVal = val.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

            if (string.IsNullOrWhiteSpace(fieldsScreener))
                return preparedVal.Replace(fieldsSeparator, " ");

            preparedVal = preparedVal.Replace(fieldsScreener, fieldsScreener + fieldsScreener);

            return fieldsScreener + preparedVal + fieldsScreener;
        }

        /// <summary>
        /// Exports leads with Uids passed over to a TSV file. Returns a string containing all data.
        /// </summary>
        /// 
        /// <param name="userUid">UID of the user for which to retrieve ID of the default configuration</param>
        /// <param name="leadUids">List of Uids for lead records that should be exported. All leads are exported if null or empty</param>
        /// <param name="format">Export file format</param>
        /// 
        /// <returns>String containing dump for all the records</returns>
        private async Task<byte[]> ExportLeadsToCsv(Guid userUid, List<Guid> leadUids, string format)
        {
            var leadsDump = "";

            var fieldsScreener = FieldsScreener;
            var fieldsSeparator = FieldsSeparator;

            if (format == "excel")
            {
                fieldsScreener = "";
                fieldsSeparator = "\t";
            }

            try
            {
                using (var db = new AvendDbContext(DbOptions))
                {
                    var leadsQuery = db.LeadsTable.Include(lead => lead.Event)
                        .Include(lead => lead.QuestionAnswers)
                        .Where(record => record.UserUid == userUid);
                    if (leadUids != null)
                        leadsQuery = leadsQuery.Where(record => leadUids.Contains(record.Uid));

                    leadsDump = LeadMappingHelper.LeadCrmExportPropertiesMap.Select(pi => pi.Key).Aggregate(
                        (result, val) =>
                            $"{result}{fieldsSeparator}{GetExportFileFormattedField(val, fieldsSeparator, fieldsScreener)}"
                    );
                    leadsDump += $"{fieldsSeparator}EventName{fieldsSeparator}EventCity{fieldsSeparator}EventStartDate";

                    foreach (var lead in await leadsQuery.ToListAsync())
                    {
                        leadsDump += "\n" +
                                     LeadMappingHelper.LeadCrmExportPropertiesMap.Select(pi => pi.Value.GetValue(lead))
                                         .Aggregate(
                                             (result, val) =>
                                             {
                                                 var propValue = val != null ? val.ToString() : "";

                                                 return result +
                                                        $"{fieldsSeparator}{GetExportFileFormattedField(propValue, fieldsSeparator, fieldsScreener)}";
                                             });

                        if (lead.Event != null)
                        {
                            var startDate = lead.Event.StartDate.HasValue
                                ? lead.Event.StartDate.Value.ToString("D")
                                : "";

                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{lead.Event.Name}{fieldsScreener}";
                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{lead.Event.City}{fieldsScreener}";
                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{startDate}{fieldsScreener}";
                        }
                        else
                        {
                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{fieldsScreener}";
                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{fieldsScreener}";
                            leadsDump += $"{fieldsSeparator}{fieldsScreener}{fieldsScreener}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical("Encountered exception when trying to export leads to TSV file: " + ex.Message);
            }

            return Encoding.UTF8.GetBytes(leadsDump);
        }

        private async Task<byte[]> ExportLeadsToExcel(Guid userUid, List<Guid> leadUids)
        {
            var memory = new MemoryStream();
            using (var db = new AvendDbContext(DbOptions))
            using (var excel = new ExcelPackage())
            {
                var leadsQuery = db.LeadsTable
                    .Include(lead => lead.Event)
                    .Include(lead => lead.QuestionAnswers)
                    .Where(record => record.UserUid == userUid);

                if (leadUids != null)
                    leadsQuery = leadsQuery.Where(record => leadUids.Contains(record.Uid));

                var sheet = excel.Workbook.Worksheets.Add("Avend Leads");

                //fill header
                var col = 1;
                var row = 1;
                foreach (var kv in LeadMappingHelper.ExportableProperties)
                {
                    sheet.Cells[row, col++].Value = kv.Name;
                }
                sheet.Cells[row, col++].Value = "EventName";
                sheet.Cells[row, col++].Value = "EventCity";
                sheet.Cells[row, col].Value = "EventStartDate";
                //add leads data
                foreach (var lead in await leadsQuery.ToListAsync())
                {
                    row++;
                    col = 1;
                    foreach (var prop in LeadMappingHelper.ExportableProperties)
                    {
                        var value = prop.GetValue(lead);
                        sheet.Cells[row, col++].Value = value?.ToString();
                    }
                    if (lead.Event != null)
                    {
                        var startDate = lead.Event.StartDate.HasValue
                            ? lead.Event.StartDate.Value.ToString("D")
                            : "";
                        sheet.Cells[row, col++].Value = lead.Event.Name;
                        sheet.Cells[row, col++].Value = lead.Event.City;
                        sheet.Cells[row, col].Value = startDate;
                    }
                }

                excel.Save();
                excel.Stream.Seek(0, SeekOrigin.Begin);
                await excel.Stream.CopyToAsync(memory);
                return memory.ToArray();
            }
        }

        public async Task<byte[]> PrepareExportData(LeadsExportRequestDto leadExportRequestDto)
        {
            Assert.Argument(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();
            if (leadExportRequestDto.Format == null) leadExportRequestDto.Format = "csv";            
            Assert.Argument(ExtensionsMapping.ContainsKey(leadExportRequestDto.Format), "format")
                .IsTrue("Only csv/excel formats are supported");

            Assert.Argument(leadExportRequestDto, "lead_uids").NotNull("Cannot parse list of GUIDs to be exported");

            List<Guid> preparedLeadUids;
            if (leadExportRequestDto.Uids?.Any() != true)
                preparedLeadUids = null;
            else
                preparedLeadUids =
                    leadExportRequestDto.Uids.Where(record => record.HasValue).Select(record => record.Value).ToList();

            Logger.LogInformation("Preparing export data for user " + UserContext.UserUid + " and lead uids: " +
                                  JsonConvert.SerializeObject(preparedLeadUids));

            if (leadExportRequestDto.Format == "excel")
            {
                return await ExportLeadsToExcel(UserContext.UserUid, preparedLeadUids);
            }
            else if (leadExportRequestDto.Format == "csv")
            {
                return await ExportLeadsToCsv(UserContext.UserUid, preparedLeadUids, leadExportRequestDto.Format);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Constructs proper leads file name based on current date and time.
        /// Extensions is chosen based on export format.
        /// </summary>
        /// 
        /// <param name="format">Export file format</param>
        /// 
        /// <returns>String containing constructed file name</returns>
        public string ConstructExportFileName(string format)
        {
            Assert.Argument(UserContext, nameof(UserContext)).NotNull();
            Assert.Argument(UserContext.UserUid, nameof(UserContext)).NotNull();
            Assert.Argument(UserContext.Member, nameof(UserContext)).NotNull("Cannot get user account details");
            Assert.Argument(format, nameof(format)).NotNull();
            Assert.Argument(ExtensionsMapping.ContainsKey(format), nameof(format))
                .IsTrue("Only csv/excel formats are supported");

            var fullName = UserContext.Member.FirstName + " " + UserContext.Member.LastName;
            var filteredFullName = fullName.Replace("\\", "").Replace("/", "").Replace(" ", "_");
            if (string.IsNullOrWhiteSpace(filteredFullName))
                filteredFullName = "User";

            var file = $"Avend-{filteredFullName}-lead_export-{DateTime.UtcNow:dd.MM.yyyy}.{ExtensionsMapping[format]}";

            Logger.LogInformation("Exporting data into file: " + file);

            return file;
        }
    }
}