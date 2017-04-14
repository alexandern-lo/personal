using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avend.API.Model;
using Newtonsoft.Json;

namespace Avend.API.Services.Helpers
{
    /// <summary>
    /// Static class to encapsulate the defaults for CRM systems.
    /// </summary>
    public static class CrmDefaultsHelper
    {
        /// <summary>
        /// Sample method that gets lead metadata from Dynamics365 crm
        /// </summary>
        /// 
        /// <returns>API response body</returns>
        public static async Task<string> RetrieveDynamics365LeadMetadata()
        {
            const string bearerToken = "00D0Y000000aA9b!ARcAQENKEEYx0bZjT5ToBcctmF5mcQVaqwpPsWpeBuEzJrpPN_kX2QgZC4cGMpOFwO8KMSzEjQgQi5ZHxOIpn2go8YzqYq6y";

            var httpResponseBody = await Dynamics365Get("/api/data/v8.2/EntityDefinitions(LogicalName='lead')", bearerToken);

            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpResponseBody);

            //Logger.LogWarning("Received from Dynamics365 data:\n" + JsonConvert.SerializeObject(data, Formatting.Indented));

            return httpResponseBody;
        }

        /// <summary>
        /// Auxillary method to process GET requests to Dynamics365 API
        /// </summary>
        /// 
        /// <param name="requestUrl"></param>
        /// <param name="bearerToken"></param>
        /// 
        /// <returns></returns>
        private static async Task<string> Dynamics365Get(string requestUrl, string bearerToken)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://d365extra.crm.dynamics.com")
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                requestUrl
                );

            request.Content = new StringContent("");
            request.Content.Headers.ContentType.MediaType = "application/json";
            request.Content.Headers.ContentType.CharSet = "utf-8";
            request.Content.Headers.Add("OData-MaxVersion", "4.0");
            request.Content.Headers.Add("OData-Version", "4.0");

            var httpResponseMessage = await client.SendAsync(request);

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            return httpResponseBody;
        }

        /// <summary>
        /// Sample method that gets lead metadata from SalesForce CRM
        /// </summary>
        /// 
        /// <returns>API response body</returns>
        public static async Task<string> RetrieveSalesForceLeadMetadata()
        {
            //  https://na35.salesforce.com/services/data/v20.0/
            //  https://eu11.salesforce.com/services/data/v20.0/ -H 'Authorization: Bearer 00D0Y000000aA9b!ARcAQENKEEYx0bZjT5ToBcctmF5mcQVaqwpPsWpeBuEzJrpPN_kX2QgZC4cGMpOFwO8KMSzEjQgQi5ZHxOIpn2go8YzqYq6y'

            var client = new HttpClient
            {
                BaseAddress = new Uri("https://eu11.salesforce.com/services/data/v20.0")
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            const string bearerToken = "00D0Y000000aA9b!ARcAQENKEEYx0bZjT5ToBcctmF5mcQVaqwpPsWpeBuEzJrpPN_kX2QgZC4cGMpOFwO8KMSzEjQgQi5ZHxOIpn2go8YzqYq6y";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                ""
                );

            request.Content = new StringContent("");
            request.Content.Headers.ContentType.MediaType = "application/json";
            request.Content.Headers.ContentType.CharSet = "utf-8";

            var httpResponseMessage = await client.SendAsync(request);

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            return httpResponseBody;
        }

        public static readonly CrmSystemAbbreviation[] AvailableCrmSystemAbbreviations = {
            CrmSystemAbbreviation.Salesforce,
            CrmSystemAbbreviation.Dynamics365,
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string> CrmNames = new Dictionary<CrmSystemAbbreviation, string>()
        {
            {CrmSystemAbbreviation.Salesforce, "Salesforce"},
            {CrmSystemAbbreviation.Dynamics365, "Dynamics365"},
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string> CrmAuthorizationUrls = new Dictionary<CrmSystemAbbreviation, string>()
        {
            {CrmSystemAbbreviation.Salesforce, "https://login.salesforce.com/services/oauth2/authorize"},
            {CrmSystemAbbreviation.Dynamics365, "https://login.microsoftonline.com/{tenant_uid}/oauth2/authorize"},
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string[]> CrmAuthorizationParams = new Dictionary<CrmSystemAbbreviation, string[]>()
        {
            {CrmSystemAbbreviation.Salesforce, new[]
            {
                "client_id={client_id}",
                "redirect_uri={return_url}",
                "grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer",
                "response_type=code",
                "scope=openid api refresh_token offline_access",
                "refresh_token={refresh_token}",
            }},
            {CrmSystemAbbreviation.Dynamics365, new[]
            {
                "response_type=code",
                "resource={api_url}",
                "client_id={client_id}",
                "redirect_uri={return_url}",
                "response_mode=query",
            }},
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string> CrmTokenRequestUrls = new Dictionary<CrmSystemAbbreviation, string>()
        {
            {CrmSystemAbbreviation.Salesforce, "https://login.salesforce.com/services/oauth2/token"},
            {CrmSystemAbbreviation.Dynamics365, "https://login.windows.net/common/oauth2/token"},
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string[]> CrmTokenRefreshRequestParams = new Dictionary<CrmSystemAbbreviation, string[]>()
        {
            {
                CrmSystemAbbreviation.Salesforce,
                new[]
                {
                    "client_id={client_id}",
                    "client_secret={client_secret}",
                    "redirect_uri={return_url}",
                    "grant_type=refresh_token",
                    "refresh_token={refresh_token}",
                }
            },
            {CrmSystemAbbreviation.Dynamics365, new[]
            {
                "grant_type=refresh_token",
                "resource={api_url}",
                "client_id={client_id}",
                "redirect_uri={return_url}",
                "refresh_token={refresh_token}",
                "client_secret={client_secret}"
            }},
        };

        public static readonly Dictionary<CrmSystemAbbreviation, string[]> CrmTokenRequestParams = new Dictionary<CrmSystemAbbreviation, string[]>()
        {
            {CrmSystemAbbreviation.Salesforce, new[]
            {
                "client_id={client_id}",
                "client_secret={client_secret}",
                "redirect_uri={return_url}",
                "grant_type=authorization_code",
                "code={grant_code}",
            }},
            {CrmSystemAbbreviation.Dynamics365, new[]
            {
                "grant_type=authorization_code",
                "resource={api_url}",
                "client_id={client_id}",
                "redirect_uri={return_url}",
                "code={grant_code}",
                "client_secret={client_secret}"
            }},
        };


        /// <summary>
        /// Map from avend db column to CRM specific field
        /// </summary>
        public static readonly Dictionary<CrmSystemAbbreviation, Dictionary<string, string>> DefaultCrmMappings = new Dictionary<CrmSystemAbbreviation, Dictionary<string, string>>()
        {
            {
                CrmSystemAbbreviation.Salesforce, new Dictionary<string, string>()
                {
/*
                    {"lead_uid", "Id"},
                    {"user_uid", "UserUid"},
*/
                    {"notes", "Description"},

                    {"first_name", "FirstName"},
                    {"last_name", "LastName"},
                    {"company_name", "Company"},
                    {"company_url", "Website"},
                    {"job_title", "Title"},
                    {"zip_code", "PostalCode"},
                    {"address", "Street"},
                    {"city", "City"},
                    {"state", "State"},
                    {"country", "Country"},
/*
                    {"location_string", "FirstEntryLocation"},
                    {"location_latitude", "FirstEntryLocationLatitude"},
                    {"location_longitude", "FirstEntryLocationLongitude"},
*/
                    {"qualification", "Rating"},
                    {"work_phone1", "Phone"},
                    {"mobile_phone1", "MobilePhone"},
                    {"email1", "Email"},
/*
                    {"exported_at", "ExportStatuses"},
                    {"created_at", "CreatedDate"},
                    {"updated_at", "LastModifiedDate"},
*/
                }
            },
            {
                CrmSystemAbbreviation.Dynamics365, new Dictionary<string, string>()
                {
                    
                    {"lead_uid", "leadid"},
/*                    {"user_uid", "UserUid"},
*/
                    {"notes", "description"},

                    {"first_name", "firstname"},
                    {"last_name", "lastname"},
                    {"company_name", "companyname"},
                    {"company_url", "websiteurl"},
                    {"job_title", "jobtitle"},
                    {"zip_code", "address1_postalcode"},
                    {"address", "address1_line1"},
                    {"city", "address1_city"},
                    {"state", "address1_stateorprovince"},
                    {"country", "address1_country"},
/*
                    {"location_string", "FirstEntryLocation"},
                    {"location_latitude", "FirstEntryLocationLatitude"},
                    {"location_longitude", "FirstEntryLocationLongitude"},
*/
                    {"qualification", "qualificationcomments"},
                    {"work_phone1", "telephone2"},
                    {"mobile_phone1", "telephone1"},
                    {"email1", "emailaddress1"},

                    /*{"photo_url", "entityimage_url" }*/
/*
                    {"exported_at", "ExportStatuses"},
                    {"created_at", "CreatedDate"},
                    {"updated_at", "LastModifiedDate"},
*/
                }
            }
        };
    }
}
