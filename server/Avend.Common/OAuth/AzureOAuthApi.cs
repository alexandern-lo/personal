using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qoden.Validation;

namespace Avend.OAuth
{
    public class AzureOAuthApi : OAuthApi
    {
        public static readonly Uri AzureCommonTokenUrl = new Uri("https://login.windows.net/common/oauth2/token");
        public static readonly Uri AzureCommonAuthUrl = new Uri("https://login.windows.net/common/oauth2/authorize");
        public const string IdToken = "id_token";

        public AzureOAuthApi(OAuthConfig config) : base(config, AzureCommonTokenUrl, AzureCommonAuthUrl)
        {
        }

        public void SetTenantId(string tenantId)
        {
            Assert.Argument(tenantId, nameof(tenantId)).NotNull();
            TokenUrl = new Uri($"https://login.windows.net/{tenantId}/oauth2/token");
            AuthUrl = new Uri($"https://login.windows.net/{tenantId}/oauth2/authorize");
        }
    }
}
