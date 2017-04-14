using System;
using System.Collections.Generic;
using Avend.API.Model;
using Avend.OAuth;
using System.Threading.Tasks;
using Avend.ApiTests.Infrastructure;
using Avend.ApiTests.Infrastructure.Extensions;
using Avend.API;
using Avend.API.Services.Helpers;
using Newtonsoft.Json;

namespace Avend.ApiTests.DataSamples
{
    public class CrmData
    {
        public static CrmData Init(TestUser user, TestSystem system)
        {
            return new CrmData
            {
                User = user,
                System = system,
                SalesForceConfig = Startup.ReadOAuthConfig(system.Configuration.GetSection("Authentication:SalesForce")),
                DynamicsConfig = Startup.ReadOAuthConfig(system.Configuration.GetSection("Authentication:Dynamics365"))
            };
        }

        public TestUser User { get; private set; }
        public TestSystem System { get; private set; }
        public OAuthConfig SalesForceConfig { get; private set; }
        public OAuthConfig DynamicsConfig { get; private set; }

        public readonly Dictionary<string, bool> SalesForceFieldMappings = new Dictionary<string, bool>()
        {
            {"first_name", true},
            {"last_name", true},
            {"work_phone1", true},
            {"email1", true},
            {"company_name", true }
        };

        public readonly Dictionary<string, bool> Dynamics365FieldMappings = new Dictionary<string, bool>()
        {
            {"first_name", true},
            {"company_name", true},
        };

        public async Task<CrmRecord> AuthorizeDynamics()
        {
            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                var dynamicsOAuthApi = new AzureOAuthApi(DynamicsConfig);
                var username = "alexandern@avendinc.onmicrosoft.com";
                var password = "Beacons1!";
                var dynamicsUrl = "https://avendinc.crm.dynamics.com";
                var dynamicsResponse = await dynamicsOAuthApi.LoginWithUsernamePassword(username, password,
                    new Dictionary<string, string>()
                    {
                        {"resource", dynamicsUrl}
                    });
                var config = new CrmRecord
                {
                    Uid = Guid.NewGuid(),
                    UserUid = User.Uid,
                    Name = "Sample Dynamics365 Config",
                    CrmType = CrmSystemAbbreviation.Dynamics365,
                    Url = dynamicsUrl,
                    AccessToken = dynamicsResponse[OAuthApi.AccessToken] as string,
                    RefreshToken = dynamicsResponse[OAuthApi.RefreshToken] as string,
                    SyncFields = JsonConvert.SerializeObject(Dynamics365FieldMappings),
                };
                db.Crms.Add(config);
                await db.SaveChangesAsync();
                return config;
            }
        }

        public async Task<CrmRecord> AuthorizeSalesForce()
        {
            using (var services = System.GetServices())
            {
                var db = services.GetService<AvendDbContext>();
                var salesForceOauthApi = new SalesForceOAuthApi(SalesForceConfig);
                const string username = "apps@studiomobile.ru";
                const string password = "qweASD123";
                const string token = "ClgBUa7O53myf6xq6f9Zt5hg";
                var response = await salesForceOauthApi.LoginWithUsernamePassword(username, password + token);
                var config = new CrmRecord
                {
                    Uid = Guid.NewGuid(),
                    UserUid = User.Uid,
                    Name = "Sample SalesForce Config",
                    CrmType = CrmSystemAbbreviation.Salesforce,
                    AccessToken = response[OAuthApi.AccessToken] as string,
                    RefreshToken =
                        string.Format("USERNAME:PASSWORD {{'username':'{0}', 'password':'{1}{2}'}}", username,
                            password, token),
                    SyncFields = JsonConvert.SerializeObject(SalesForceFieldMappings),
                    Url = (response[SalesForceOAuthApi.InstanceUrl] as string) + "/services/data/v20.0/"
                };

                db.Crms.Add(config);
                await db.SaveChangesAsync();
                return config;
            }
        }
    }
}