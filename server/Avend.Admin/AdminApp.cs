using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Avend.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qoden.Validation;

namespace Avend.Admin
{
    public class AdminApp
    {
        private readonly AppConfig _config;
        private string _accessToken;
        private string _isAdminPropName;


        public AdminApp(AppConfig config)
        {
            Assert.Argument(config, nameof(config)).NotNull();
            _config = config;
        }

        public async Task<bool> Login()
        {
            var azure = new AzureOAuthApi(_config.UsersManagement);
            azure.SetTenantId(_config.TenantUid);
            var response = await azure.LoginWithClientCredentials(new Dictionary<string, string>
            {
                {"resource", "https://graph.windows.net"}
            });
            _accessToken = response[OAuthApi.AccessToken] as string;
            return _accessToken != null;
        }

        private HttpClient CreateClient()
        {
            Assert.State(_accessToken).NotNull();
            var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            http.BaseAddress = new Uri($"https://graph.windows.net/{_config.TenantName}/");
            return http;
        }

        public async Task<JArray> ListExtensionProps()
        {
            using (var http = CreateClient())
            {
                var json = JsonConvert.SerializeObject(new {isSyncedFromOnPremises = false});
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await http.PostAsync($"getAvailableExtensionProperties?api-version=1.6", content);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var obj = JsonConvert.DeserializeObject<JObject>(body);
                    return obj["value"].Value<JArray>();
                }
                throw new AdminAppException(response, body);
            }
        }

        public async Task<string> LoadIsAdminPropName()
        {
            if (_isAdminPropName == null)
            {
                var props = await ListExtensionProps();
                foreach (var p in props)
                {
                    var propObj = p as JObject;
                    if (propObj != null)
                    {
                        var name = propObj.Value<string>("name");
                        if (name != null && name.EndsWith("IsAdmin"))
                        {
                            _isAdminPropName = name;
                        }
                        break;
                    }
                }
            }
            return _isAdminPropName;
        }

        public async Task GrantAdmin(string userId, bool admin)
        {
            using (var http = CreateClient())
            {
                var adminPropName = await LoadIsAdminPropName();
                Assert.State(adminPropName, nameof(adminPropName)).NotNull();
                var update = new Dictionary<string, object>
                {
                    {adminPropName, admin ? "Yes" : "No"}
                };
                var json = JsonConvert.SerializeObject(update);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"users/{userId}?api-version=1.6")
                {
                    Content = content
                };
                var response = await http.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new AdminAppException(response, body);
                }
            }
        }

        public async Task<JObject> GetUser(string id)
        {
            using (var http = CreateClient())
            {
                var filter = $"startswith(displayName, '{id}') or " +
                             $"startswith(mailNickname, '{id}') or " +
                             $"startswith(mail, '{id}') or " +
                             $"signInNames/any(n: n/value eq '{id}') or " +
                             $"otherMails/any(n: n eq '{id}') or " +
                             $"startswith(userPrincipalName, '{id}')";
                var response = await http.GetAsync($"users?api-version=1.6&$filter={filter}");
                var body = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new AdminAppException(response, body);
                }
                return JsonConvert.DeserializeObject<JObject>(body);
            }
        }
    }
}