using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Model;
using Avend.API.Services.Helpers;
using Avend.OAuth;
using Newtonsoft.Json;
using Qoden.Validation;

namespace Avend.ApiTests.DataSamples
{
    public class CrmSystems
    {
        public const string DynamicsUserName = "KonstantinK@liveoakinc.com";
        public const string DynamicsPassword = "Fi=fa-8080";
        public const string DynamicsUrl = "https://liveoakinc.crm.dynamics.com";

        public const string SalesForceUserName = "kkb_ru@hotmail.com";
        public const string SalesForcePassword = "09eb-nv8vdu=vcs";
        public const string SalesForceSecurityToken = "npEKYyyB84ksNpHKMlLsJPmow";
        private static string _salesForceApiUrl;
        public string SalesForceApiUrl => _salesForceApiUrl;

        private static string _salesForceAccessToken;
        //SalesForce OAuth username/password flow does not provide refresh token :(
        private static string _salesForceRefreshToken = null;
        private static string _dynamicsAccessToken;
        private static string _dynamicsRefreshToken;

        public static async Task<CrmSystems> Create()
        {
            if (_salesForceAccessToken == null)
            {
                var salesForceOAuthApi = new SalesForceOAuthApi(_salesForceOAuthConfig);
                var loginResponse = await salesForceOAuthApi.LoginWithUsernamePassword(SalesForceUserName, SalesForcePassword + SalesForceSecurityToken);
                _salesForceAccessToken = loginResponse[OAuthApi.AccessToken] as string;
                _salesForceApiUrl = loginResponse["instance_url"] as string;
            }

            if (_dynamicsAccessToken == null)
            {
                var dynamicsOAtuhApi = new AzureOAuthApi(_dynamicsOAuthConfig);
                var response = await dynamicsOAtuhApi.LoginWithUsernamePassword(DynamicsUserName, DynamicsPassword,
                    new Dictionary<string, string>()
                    {
                        {"resource", DynamicsUrl}
                    });
                _dynamicsAccessToken = response[OAuthApi.AccessToken] as string;
                _dynamicsRefreshToken = response[OAuthApi.RefreshToken] as string;
            }
            return new CrmSystems();
        }

        private CrmSystems()
        {
            Assert.State(_dynamicsAccessToken, "DynamicsAccessToken").NotNull();
            Assert.State(_dynamicsRefreshToken, "DynamicsRefreshToken").NotNull();
            Assert.State(_salesForceAccessToken, "SalesForceAccessToken").NotNull();
            SalesForceOAuthConfig = new OAuthConfig()
            {
                ClientId = _salesForceOAuthConfig.ClientId,
                ClientSecret = _salesForceOAuthConfig.ClientSecret,
                ReturnUrl = _salesForceOAuthConfig.ReturnUrl
            };

            DynamicsOAuthConfig = new OAuthConfig()
            {
                ClientId = _dynamicsOAuthConfig.ClientId,
                ClientSecret = _dynamicsOAuthConfig.ClientSecret,
                ReturnUrl = _dynamicsOAuthConfig.ReturnUrl
            };

            SalesForceFieldMappings = new Dictionary<string, object>()
            {
                {"first_name", "first_name"},
                {"last_name", "last_name"},
            };
            Dynamics365FieldMappings = new Dictionary<string, object>()
            {
                {"first_name", "firstname"},
                {"last_name", "lastname"},
            };

            SalesForce = new CrmSystem
            {
                Abbreviation = CrmSystemAbbreviation.Salesforce,
                AuthorizationParams =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.CrmAuthorizationParams[CrmSystemAbbreviation.Salesforce]),
                TokenRequestUrl = CrmDefaultsHelper.CrmTokenRequestUrls[CrmSystemAbbreviation.Salesforce],
                TokenRequestParams =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.CrmTokenRequestParams[CrmSystemAbbreviation.Salesforce]),
                DefaultFieldMappings =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.DefaultCrmMappings[CrmSystemAbbreviation.Salesforce]),
            };

            Dynamics365 = new CrmSystem
            {
                Abbreviation = CrmSystemAbbreviation.Dynamics365,
                AuthorizationParams =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.CrmAuthorizationParams[CrmSystemAbbreviation.Dynamics365]),
                TokenRequestUrl = CrmDefaultsHelper.CrmTokenRequestUrls[CrmSystemAbbreviation.Dynamics365],
                TokenRequestParams =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.CrmTokenRequestParams[CrmSystemAbbreviation.Dynamics365]),
                DefaultFieldMappings =
                    JsonConvert.SerializeObject(
                        CrmDefaultsHelper.DefaultCrmMappings[CrmSystemAbbreviation.Dynamics365]),
            };
        }

        public string DynamicsAccessToken => _dynamicsAccessToken;
        public string DynamicsRefreshToken => _dynamicsRefreshToken;
        public string SalesForceAccessToken => _salesForceAccessToken;
        public string SalesForceRefreshToken => _salesForceRefreshToken;

        public OAuthConfig SalesForceOAuthConfig { get; private set; }
        public OAuthConfig DynamicsOAuthConfig { get; private set; }
        public Dictionary<string, object> SalesForceFieldMappings { get; private set; }
        public Dictionary<string, object> Dynamics365FieldMappings { get; private set; }
        public CrmSystem SalesForce { get; private set; }
        public CrmSystem Dynamics365 { get; private set; }        

        private static readonly OAuthConfig _salesForceOAuthConfig = new OAuthConfig()
        {
            ClientSecret = "7064917936388342785",
            ClientId = "3MVG9szVa2RxsqBbyXauZXaz1rAMVO9Hmvt8I5VsRK7jtQCQ.VnAmAF45HjxVuY9t7QN.AY7qWCSm2RRUGclt",
            ReturnUrl = "https://localhost:3000/salesforce_response",
        };

        private static readonly OAuthConfig _dynamicsOAuthConfig = new OAuthConfig()
        {
            ClientId = "07ffdfab-c411-446e-b3e9-60869cccd191",
            ClientSecret = "sHtdv8ymF4rHo1vCW7ccM6Uik4M/c+3CpCb+wt+4MDY=",
            ReturnUrl = "https://localhost:3000/dynamics365_response"
        };
    }
}