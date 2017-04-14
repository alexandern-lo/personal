using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services.Crm;
using Avend.API.Services.Exceptions;
using Avend.API.Services.Leads.NetworkDTO;
using Avend.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class LeadsExportToCrmService
    {
        private static readonly string UsernamePasswordTokenScheme = "USERNAME:PASSWORD";

        private ILogger Logger { get; }

        public UserContext UserContext { get; }
        public UsersManagementService UsersManagementService { get; }

        public DbContextOptions<AvendDbContext> DbOptions { get; }

        public AppSettingsCrm AppSettingsCrm { get; }
        public CrmConnectorFactory CrmConnectorFactory { get; }

        /// <summary>
        /// Default constructor.
        /// 
        /// Instantiates logger using AvendLog.
        /// </summary>
        public LeadsExportToCrmService(
            UserContext userContext,
            AppSettingsCrm appSettingsCrm,
            DbContextOptions<AvendDbContext> dbOptions,
            UsersManagementService usersManagementService
        )
        {
            Assert.Argument(userContext, nameof(userContext)).NotNull();
            Assert.Argument(dbOptions, nameof(dbOptions)).NotNull();
            Assert.Argument(usersManagementService, nameof(usersManagementService)).NotNull();
            Assert.Argument(appSettingsCrm, nameof(appSettingsCrm)).NotNull();

            AppSettingsCrm = appSettingsCrm;

            UserContext = userContext;
            UsersManagementService = usersManagementService;

            DbOptions = dbOptions;

            CrmConnectorFactory = new CrmConnectorFactory(AppSettingsCrm);

            Logger = AvendLog.CreateLogger<LeadsExportToFileService>();
        }

        /// <summary>
        /// Export leads to CRM one by one.
        /// </summary>
        /// <param name="crmConnector"></param>
        /// <param name="crmRecord"></param>
        /// <param name="userUid"></param>
        /// <param name="leadUids"></param>
        /// 
        /// <returns>Async List of <see cref="LeadExportResult"/> objects, one per each lead.</returns>
        /// 
        /// <exception cref="CrmUnauthorizedException">When Crm returns 401/403 response we throw this exception.</exception>
        private async Task<List<LeadExportResult>> ExportLeadsOfUserByUids(BaseCrmConnector crmConnector,
            CrmRecord crmRecord, Guid userUid, List<Guid> leadUids)
        {
            var report = new List<LeadExportResult>();
            Dictionary<string, object> tokens;
            try
            {
                var refreshToken = crmRecord.RefreshToken;

                if (refreshToken != null && refreshToken.StartsWith(UsernamePasswordTokenScheme))
                {
                    var jsonStr = refreshToken.Substring(UsernamePasswordTokenScheme.Length);
                    var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                    tokens = await crmConnector.GetAccessCodeUsingIdAndPassword(json["username"], json["password"]);
                }
                else
                {
                    tokens = await crmConnector.GetAccessCodeUsingRefreshToken(crmRecord);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(LoggingEvents.LEAD_EXPORT, ex, "Token retrieval failed");
                var message = "Cannot get access token: " + ex.Message;
/*
                report = leadUids.Select(leadUid =>
                {
                    return new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Failed,
                        Message = message,
                        LeadUid = leadUid,
                        Lead = null,
                        ProcessedAt = DateTime.UtcNow,
                        IsRecoverable = false,
                    };
                }).ToList();
*/

                throw new CrmUnauthorizedException(message, ex);
            }

            using (var db = new AvendDbContext(DbOptions))
            {
                if (leadUids == null || leadUids.Count == 0)
                {
                    Logger.LogWarning(LoggingEvents.LEAD_EXPORT, "Requested export of all Leads from database");

                    leadUids = db.LeadsTable
                        .Where(record => record.UserUid == userUid)
                        .Select(record => record.Uid)
                        .ToList();
                }

                crmRecord.AccessToken = tokens[OAuthApi.AccessToken] as string;

                if (crmRecord.CrmType == CrmSystemAbbreviation.Salesforce)
                    crmRecord.Url = tokens[SalesForceOAuthApi.InstanceUrl] as string +
                                                      "/services/data/v20.0/";

                var leadsQuery = db.LeadsTable.Include(record => record.ExportStatuses)
                    .Include(record => record.Emails)
                    .Include(record => record.Phones)
                    .Where(UserContext.AvailableLeads())
                    .Where(x => leadUids.Contains(x.Uid));

                var unprocessedUids = new List<Guid>(leadUids);

                LeadRecord lastProcessedLead = null;
                try
                {
                    foreach (var lead in leadsQuery)
                    {
                        lastProcessedLead = lead;
                        Logger.LogWarning(LoggingEvents.LEAD_EXPORT, "Found Uid in database: {0}", lead.Uid);
                        unprocessedUids.Remove(lead.Uid);
                        report.Add(await crmConnector.UploadLead(lead, crmRecord));
                    }

                    Logger.LogWarning(LoggingEvents.LEAD_EXPORT, "Not found Uids in database: {}",
                        JsonConvert.SerializeObject(unprocessedUids));
                }
                catch (Exception ex)
                {
                    report.Add(new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Failed,
                        IsRecoverable = false,
                        LeadUid = lastProcessedLead?.Uid ?? Guid.Empty,
                        LeadRecord = lastProcessedLead,
                        ProcessedAt = DateTime.Now,
                        Message = "Exception: " + ex.Message,
                    });

                    Logger.LogError(LoggingEvents.LEAD_EXPORT, ex, "ExportLeadsToCrm failed with error");
                }

                foreach (var leadUid in unprocessedUids)
                {
                    report.Add(new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Failed,
                        IsRecoverable = false,
                        LeadUid = leadUid,
                        LeadRecord = null,
                        ProcessedAt = DateTime.Now,
                        Message = "Not processed",
                    });
                }

                await db.SaveChangesAsync();
            }

            return report;
        }

        /// <summary>
        /// Main function responsible for export to CRMs.
        /// Validates input data, performs export, generates report.
        /// </summary>
        /// 
        /// <param name="leadExportRequestDto">DTO describing which lead records to export</param>
        /// 
        /// <returns>DTO with report on export results</returns>
        /// 
        /// <exception cref="CrmUnauthorizedException">Throws exception if CRM returns rejects refresh token and responds with 401/403</exception>
        public async Task<LeadsExportReportDto> ExportLeads(LeadsExportRequestDto leadExportRequestDto)
        {
            Assert.Argument(UserContext.UserUid, nameof(UserContext.UserUid)).NotNull();

            Assert.Argument(leadExportRequestDto, "lead_uids").NotNull("Cannot parse list of GUIDs to be exported");

            CrmRecord crmRecord;

            var userSettings = await UsersManagementService.GetSettingsForUser(UserContext.UserUid);

            Assert.Argument(userSettings, "user_settings").NotNull("Cannot find user settings with given UID");

            if (userSettings.DefaultCrmId == null)
            {
                crmRecord =
                    UsersManagementService.Db.Crms
                        .Where(x => x.UserUid == UserContext.UserUid)
                        .OrderBy(x => x.CrmType)
                        .FirstOrDefault();
            }
            else
            {
                crmRecord = UsersManagementService.Db.Crms
                    .FirstOrDefault(x => x.UserUid == UserContext.UserUid && x.Id == userSettings.DefaultCrmId);
            }

            Assert.Argument(crmRecord, "crm_configuration ").NotNull("Cannot find valid CRM configuration");

            var crmConnector = CrmConnectorFactory.GetConnectorForCrmSystem(crmRecord.CrmType);
            var exportData = new List<LeadExportResult>();

            try
            {
                var filteredLeadUids =
                    leadExportRequestDto.Uids?.Where(record => record.HasValue).Select(record => record.Value).ToList();

                if (leadExportRequestDto.Uids?.Count > 0 && filteredLeadUids?.Count == 0)
                {
                    exportData = new List<LeadExportResult>();
                }
                else
                {
                    exportData = await ExportLeadsOfUserByUids(
                        crmConnector, crmRecord, UserContext.UserUid, filteredLeadUids
                    );
                }
            }
            catch (CrmUnauthorizedException ex)
            {
                Logger.LogCritical("CrmUnauthorizedException during export to CRM: " + ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                Logger.LogCritical("Exception during export to CRM: " + ex.Message);
            }

            var createdRecords = exportData
                .Where(record => record.Status == LeadExportResultStatus.Created)
                .Select(record => new LeadExportSuccessDetails()
                {
                    LeadUid = record.LeadUid,
                    FirstName = record.LeadRecord?.FirstName,
                    LastName = record.LeadRecord?.LastName,
                    Email = record.LeadRecord?.Emails.FirstOrDefault()?.Email,
                    ExportedAt = record.ProcessedAt,
                }).ToList();

            var updatedRecords = exportData
                .Where(record => record.Status == LeadExportResultStatus.Updated)
                .Select(record => new LeadExportSuccessDetails()
                {
                    LeadUid = record.LeadUid,
                    FirstName = record.LeadRecord?.FirstName,
                    LastName = record.LeadRecord?.LastName,
                    Email = record.LeadRecord?.Emails.FirstOrDefault()?.Email,
                    ExportedAt = record.ProcessedAt,
                }).ToList();

            var failedRecords = exportData
                .Where(record => record.Status == LeadExportResultStatus.Failed)
                .Select(record => new LeadExportFailureDetails()
                {
                    LeadUid = record.LeadUid,
                    FirstName = record.LeadRecord?.FirstName,
                    LastName = record.LeadRecord?.LastName,
                    Email = record.LeadRecord?.Emails.FirstOrDefault()?.Email,
                    FailedAt = record.ProcessedAt,
                    Reason = record.Message,
                }).ToList();

            return new LeadsExportReportDto()
            {
                TotalCreated = createdRecords.Count,
                CreatedLeads = createdRecords,
                TotalUpdated = updatedRecords.Count,
                UpdatedLeads = updatedRecords,
                TotalFailed = failedRecords.Count,
                FailedLeads = failedRecords,
            };
        }
    }
}