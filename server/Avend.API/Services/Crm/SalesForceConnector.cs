using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services.Helpers;
using Avend.OAuth;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Avend.API.Services.Crm
{
    
    public class SalesForceConnector : BaseCrmConnector
    {
        public SalesForceConnector(OAuthConfig config) : base (new SalesForceOAuthApi(config), AvendLog.CreateLogger<SalesForceConnector>())
        {
        }

        protected HttpClient MakeHttpClient(CrmRecord config)
        {
            var http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AccessToken);

            // TODO this is from SalesForce doc
            // https://developer.salesforce.com/docs/atlas.en-us.api_rest.meta/api_rest/intro_rest_resources.htm
            // Suppose you want to retrieve information about the Salesforce version. Submit a request for the Versions resource.
            // curl https://yourInstance.salesforce.com/services/data/
            // NOTE: 'yourInstance' each SF has unique name! This means ApiRootUrl completely useless for both Dynamics and SalesForce

            http.BaseAddress = new Uri(!string.IsNullOrWhiteSpace(config.Url) ? config.Url : OAuthApi.Config.ApiRootUrl);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return http;
        }

        public override async Task<Dictionary<string, object>> RetrieveLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            var status = GetLastLeadExportForCrm(crmRecord, leadRecord);
            if (status == null)
                return null;

            using (var http = MakeHttpClient(crmRecord))
            {
                var responseMessage = await http.GetAsync(new Uri("sobjects/Lead/" + status.ExternalUid, UriKind.Relative));
                var response = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
            }
        }

        /// <summary>
        /// Main method to upload lead to SalesForce.
        /// </summary>
        /// <param name="leadRecord">Lead to be exprted</param>
        /// <param name="crmRecord">User configuration of CRM to export into</param>
        /// 
        /// <returns>Export result object</returns>
        public override async Task<LeadExportResult> UploadLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            var status = GetLastLeadExportForCrm(crmRecord, leadRecord);

            var mappedLead = LeadMappingHelper.GetMappedLead(leadRecord, crmRecord);

            Logger.LogDebug("Mapped lead is:\n" + JsonConvert.SerializeObject(mappedLead, Formatting.Indented));

            var requestContent = new StringContent(JsonConvert.SerializeObject(mappedLead), Encoding.UTF8, "application/json");

            using (var http = MakeHttpClient(crmRecord))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("sobjects/Lead" + (status != null ? $"/{status.ExternalUid}" : ""), UriKind.Relative),
                    Method = status != null ? new HttpMethod("PATCH") : HttpMethod.Post,
                    Content = requestContent,
                };

                var responseMessage = await http.SendAsync(request);
                var responseBody = await responseMessage.Content.ReadAsStringAsync();

                if (status != null && string.IsNullOrWhiteSpace(responseBody))
                {
                    status.ExportedAt = DateTime.UtcNow;

                    return new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Updated,
                        ProcessedAt = DateTime.UtcNow,
                        LeadRecord = leadRecord,
                        LeadUid = leadRecord.Uid,
                    };
                }

                var responseObj = new Dictionary<string, object>();
                List<Dictionary<string, object>> responseList;

                Logger.LogDebug("Received resonse from Salesforce url" + http.BaseAddress + " + " + request.RequestUri + ":\n" + responseBody);

                JArray json = null;
                try
                {
                    json = JArray.Parse(responseBody);
                }
                catch (Exception)
                {
                    // ignored
                }

                if (json != null)
                {
                    responseList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(responseBody);
                }
                else
                {
                    responseObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                    responseList = new List<Dictionary<string, object>>()
                    {
                        responseObj
                    };
                }

                if (responseList != null && responseList.Count > 0)
                    responseObj = responseList[0];

                if (responseMessage.StatusCode >= HttpStatusCode.BadRequest)
                {
                    var errorCode = Convert.ToString(responseObj.ContainsKey("errorCode") ? responseObj["errorCode"] : "UNKNOWN_ERROR");

                    var message = Convert.ToString(responseObj.ContainsKey("message") ? responseObj["message"] : "No message found");

                    var result = new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Failed,
                        ProcessedAt = DateTime.UtcNow,
                        LeadRecord = leadRecord,
                        LeadUid = leadRecord?.Uid ?? Guid.Empty,
                        Message = $"Unknown error [{errorCode}]: {message}",
                        IsRecoverable = false,
                    };

                    switch (errorCode)
                    {
                        case "NOT_FOUND":
                            result.Message = message;
                            result.IsRecoverable = true;
                            break;

                        case "STRING_TOO_LONG":
                            result.Message = message;
                            break;

                        case "REQUIRED_FIELD_MISSING":
                            result.Message = message;
                            break;

                        case "METHOD_NOT_ALLOWED":
                            result.Message = message;
                            break;

                        case "UNKNOWN_ERROR":
                            result.Message = message;
                            break;
                    }

                    return result;
                }

                if (responseObj.ContainsKey("id"))
                {
                    var id = Convert.ToString(responseObj["id"]);

                    UpdateLeadExportStatus(crmRecord, leadRecord, status, id);

                    return new LeadExportResult()
                    {
                        Status = LeadExportResultStatus.Created,
                        ProcessedAt = DateTime.UtcNow,
                        LeadRecord = leadRecord,
                        LeadUid = leadRecord.Uid,
                    };
                }

                return new LeadExportResult()
                {
                    Status = LeadExportResultStatus.Failed,
                    ProcessedAt = DateTime.UtcNow,
                    LeadRecord = leadRecord,
                    LeadUid = leadRecord.Uid,
                    Message = "Unknown Salesforce error",
                };
            }
        }

        public override async Task<bool> DeleteLead(LeadRecord leadRecord, CrmRecord crmRecord)
        {
            var status = GetLastLeadExportForCrm(crmRecord, leadRecord);

            if (status == null)
                return false;
            using (var http = MakeHttpClient(crmRecord))
            {
                await http.DeleteAsync(new Uri("sobjects/Lead/" + status.ExternalUid, UriKind.Relative));
            }
            return true;
        }

        public async Task<Dictionary<string, string>> GetResourcesList(CrmRecord crmRecord)
        {
            using (var http = MakeHttpClient(crmRecord))
            {
                var responseMessage = await http.GetAsync("");
                if (responseMessage.StatusCode >= HttpStatusCode.BadRequest)
                    return null;
                var json = await responseMessage.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public async Task<List<string>> GetLeadStructure(CrmRecord crmRecord)
        {
            using (var http = MakeHttpClient(crmRecord))
            {
                var responseMessage = await http.GetAsync(new Uri("sobjects/Lead/describe", UriKind.Relative));

                if (responseMessage.StatusCode >= HttpStatusCode.BadRequest)
                    return null;

                var response = JObject.Parse(await responseMessage.Content.ReadAsStringAsync());

                var jsonFields = response["fields"];

                var fieldNames = jsonFields.Select(record => record["name"].ToString()).ToList();

                return fieldNames;
            }
        }

        /// <summary>
        /// Performs arbitrary request to the CRM's API.
        /// </summary>
        /// 
        /// <param name="method">HTTP method to be used</param>
        /// <param name="requestUrl">API endpoint's URL to get data from</param>
        /// <param name="crmRecord">CrmRecord to get access token</param>
        /// <param name="body">Request body</param>
        /// <param name="contentMediaType">Request body media type</param>
        /// <param name="extraHeaders">Extra request headers could be passed over to API if required</param>
        /// 
        /// <returns>Response body string</returns>
        private async Task<string> ApiSendRequest(HttpMethod method, string requestUrl, CrmRecord crmRecord, string body, string contentMediaType, Dictionary<string, string> extraHeaders = null)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(OAuthApi.Config.ApiRootUrl)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(method, requestUrl);
            request.Content = new StringContent(body, Encoding.UTF8, contentMediaType);

            if (extraHeaders != null && extraHeaders.Count > 0)
            {
                foreach (var extraHeader in extraHeaders)
                {
                    request.Content.Headers.Add(extraHeader.Key, extraHeader.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(crmRecord.AccessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", crmRecord.AccessToken);

            var httpResponseMessage = await client.SendAsync(request);

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            return httpResponseBody;
        }

        /// <summary>
        /// Performs GET request to the CRM's API.
        /// </summary>
        /// 
        /// <param name="requestUrl">API endpoint's URL to get data from</param>
        /// <param name="crmRecord">CrmRecord to get access token</param>
        /// <param name="extraHeaders">Extra request headers could be passed over to API if required</param>
        /// 
        /// <returns>Response body string</returns>
        protected async Task<string> ApiGetRequest(string requestUrl, CrmRecord crmRecord, Dictionary<string, string> extraHeaders = null)
        {
            return await ApiSendRequest(HttpMethod.Get, requestUrl, crmRecord, "", "application/json", extraHeaders);
        }

        /// <summary>
        /// Performs POST request to the CRM's API.
        /// </summary>
        /// 
        /// <param name="requestUrl">API endpoint's URL to get data from</param>
        /// <param name="crmRecord">CrmRecord to get access token</param>
        /// <param name="body">Request body</param>
        /// <param name="contentMediaType">Request body media type</param>
        /// <param name="extraHeaders">Extra request headers could be passed over to API if required</param>
        /// 
        /// <returns>Response body</returns>
        protected async Task<string> ApiPostRequest(string requestUrl, CrmRecord crmRecord, string body, string contentMediaType = "application/json", Dictionary<string, string> extraHeaders = null)
        {
            return await ApiSendRequest(HttpMethod.Post, requestUrl, crmRecord, body, contentMediaType, extraHeaders);
        }

        /// <summary>
        /// Performs PUT request to the CRM's API.
        /// </summary>
        /// 
        /// <param name="requestUrl">API endpoint's URL to get data from</param>
        /// <param name="crmRecord">CrmRecord to get access token</param>
        /// <param name="body">Request body</param>
        /// <param name="contentMediaType">Request body media type</param>
        /// <param name="extraHeaders">Extra request headers could be passed over to API if required</param>
        /// 
        /// <returns>Response body</returns>
        protected async Task<string> ApiPutRequest(string requestUrl, CrmRecord crmRecord, string body, string contentMediaType = "application/json", Dictionary<string, string> extraHeaders = null)
        {
            return await ApiSendRequest(HttpMethod.Put, requestUrl, crmRecord, body, contentMediaType, extraHeaders);
        }
    }
}