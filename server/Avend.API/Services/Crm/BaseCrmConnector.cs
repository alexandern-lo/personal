using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Avend.API.Model;
using Avend.OAuth;

using Microsoft.Extensions.Logging;

namespace Avend.API.Services.Crm
{
    
    /// <summary>
    /// Base class for all CRM connectors.
    /// 
    /// Implements basic API request methods and macros replacement for URL generation .
    /// </summary>
    public abstract class BaseCrmConnector
    {
        protected ILogger Logger { get; }

        /// <summary>
        /// Connector's constuctor requires CrmSystem object and CrmConfig
        /// </summary>
        /// 
        /// <param name="oauthApi">OAuth API subclass to use</param>
        /// <param name="logger">Logger to use</param>
        protected BaseCrmConnector(OAuthApi oauthApi, ILogger logger)
        {
            OAuthApi = oauthApi;

            Logger = logger;
        }

        protected LeadExportStatus GetLastLeadExportForCrm(CrmRecord crmRecord, LeadRecord leadRecord)
        {
            return leadRecord.ExportStatuses?.FirstOrDefault(record => record.UserCrmConfigurationUid == crmRecord.Uid);
        }

        public abstract Task<Dictionary<string, object>> RetrieveLead(LeadRecord leadRecord, CrmRecord crmRecord);

        public abstract Task<LeadExportResult> UploadLead(LeadRecord leadRecord, CrmRecord crmRecord);

        public abstract Task<bool> DeleteLead(LeadRecord leadRecord, CrmRecord crmRecord);

        public OAuthApi OAuthApi { get; private set; }

        /// <summary>
        /// Constructs Authorization Url for sending users to OAuth service of the CRM system.
        /// </summary>
        /// 
        /// <param name="crmRecord">User's CRM configuration, now is used to get refresh token from</param>
        /// 
        /// <returns>Url for redirecting users to OAuth-based authorization</returns>
        public virtual string ConstructAuthorizationUrl(CrmRecord crmRecord)
        {
            return OAuthApi.GetAuthorizationPageUrl().AbsoluteUri;
        }
        /// <summary>
        /// Exchange grant code to access and refresh tokens
        /// </summary>
        /// <param name="crmRecord">User crm configuration</param>
        /// <param name="grantCode">grant code</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> GetTokensByGrantCode(CrmRecord crmRecord, string grantCode)
        {
            return await OAuthApi.LoginWithGrantCode(grantCode);
        }
        /// <summary>
        /// Exchange refresh token from crmRecord to access and refresh tokens. Method will fail if CRM configuration does not have refresh token.
        /// </summary>
        /// <param name="crmRecord">user CRM configuration</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> GetAccessCodeUsingRefreshToken(CrmRecord crmRecord)
        {
            return await OAuthApi.LoginWithRefreshToken(crmRecord.RefreshToken);
        }

        protected static void UpdateLeadExportStatus(CrmRecord crmRecord, LeadRecord leadRecord, LeadExportStatus status, string externalUid)
        {
            if (status == null)
            {
                status = new LeadExportStatus()
                {
                    Uid = Guid.NewGuid(),
                    LeadId = leadRecord.Id,
                    LeadRecord = leadRecord,
                    UserCrmConfigurationUid = crmRecord.Uid,
                    ExternalUid = externalUid,
                    ExportedAt = DateTime.UtcNow,
                };

                leadRecord.ExportStatuses.Add(status);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(externalUid))
                    status.ExternalUid = externalUid;

                status.ExportedAt = DateTime.UtcNow;
            }
        }

        public async Task<Dictionary<string, object>> GetAccessCodeUsingIdAndPassword(string username, string password)
        {
            return await OAuthApi.LoginWithUsernamePassword(username, password);
        }
    }

    
}