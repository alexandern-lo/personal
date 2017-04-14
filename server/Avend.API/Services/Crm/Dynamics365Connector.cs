using Avend.API.Model;
using Avend.API.Services.Helpers;
using Avend.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Avend.API.Services.Crm
{
    public class Dynamics365Connector : BaseCrmConnector
    {
        public Dynamics365Connector(OAuthConfig crmConfig) : base(new AzureOAuthApi(crmConfig), AvendLog.CreateLogger<Dynamics365Connector>())
        {
        }

        public override string ConstructAuthorizationUrl(CrmRecord crmRecord)
        {
            return OAuthApi.GetAuthorizationPageUrl(new Dictionary<string, string>() {
                {"resource", crmRecord.Url }
            }).AbsoluteUri;
        }

        public override async Task<Dictionary<string, object>> GetTokensByGrantCode(CrmRecord crmRecord, string grantCode)
        {
            return await OAuthApi.LoginWithGrantCode(grantCode, new Dictionary<string, string>() {
                { "resource", crmRecord.Url }
            });
        }

        public override async Task<Dictionary<string, object>> GetAccessCodeUsingRefreshToken(CrmRecord crmRecord)
        {
            var body = new Dictionary<string, string>() {
                { "resource", crmRecord.Url }
            };
            return await OAuthApi.LoginWithRefreshToken(crmRecord.RefreshToken, body);
        }

        public override Task<Dictionary<string, object>> RetrieveLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            throw new NotImplementedException();
        }

        private static readonly Regex LeadId = new Regex(".*leads\\((.*)\\)");

        public override async Task<LeadExportResult> UploadLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {           
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(crmRecord.Url);
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", crmRecord.AccessToken);

                var status = GetLastLeadExportForCrm(crmRecord, leadRecord);
                var dynamicsLead = LeadMappingHelper.GetMappedLead(leadRecord, crmRecord);
                var request = new HttpRequestMessage();
                if (status == null)
                {
                    request.RequestUri = new Uri("api/data/v8.2/leads", UriKind.Relative);
                    request.Method = HttpMethod.Post;
                }
                else
                {
                    request.RequestUri = new Uri("api/data/v8.2/leads(" + status.ExternalUid + ")", UriKind.Relative);
                    request.Method = new HttpMethod("PATCH");
                }
                request.Content = new StringContent(JsonConvert.SerializeObject(dynamicsLead), Encoding.UTF8, "application/json");

                var result = new LeadExportResult()
                {
                    ProcessedAt = DateTime.Now,
                    LeadRecord = leadRecord,
                    LeadUid = leadRecord?.Uid ?? Guid.Empty,
                };

                try
                {
                    var responseMessage = await http.SendAsync(request);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var location = responseMessage.Headers.Location.AbsolutePath;
                        var match = LeadId.Match(location);
                        if (match.Success)
                        {
                            var id = match.Groups[1].Value;                            
                            result.Status = status != null ? LeadExportResultStatus.Updated : LeadExportResultStatus.Created;
                            UpdateLeadExportStatus(crmRecord, leadRecord, status, id);
                        }
                        else
                        {
                            result.Status = LeadExportResultStatus.Failed;
                            result.Message = "Dynamics API returned unknown response";
                        }                        
                    }
                    else
                    {
                        var error = responseMessage.ReasonPhrase;
                        var json = await responseMessage.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(json))
                        {
                            try
                            {
                                var response = JObject.Parse(json);
                                error = response?["error"]?["message"]?.Value<string>();
                            }
                            catch (Exception ex)
                            {
                                Logger.LogWarning(LoggingEvents.LEAD_EXPORT, ex, "Failed to parse error JSON");
                            }
                        }
                        result.Message = error;
                        result.Status = LeadExportResultStatus.Failed;                        
                    }
                }
                catch (HttpRequestException e)
                {
                    result.Status = LeadExportResultStatus.Failed;
                    result.Message = e.Message;
                    result.IsRecoverable = true;
                    Logger.LogWarning(LoggingEvents.LEAD_EXPORT, e, "Dynamics365 request failed");
                }
                return result;
            }            
        }

        public override Task<bool> DeleteLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            throw new NotImplementedException();
        }
    }
}
