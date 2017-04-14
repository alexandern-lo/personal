using System;

namespace Avend.OAuth
{
    public class SalesForceOAuthApi : OAuthApi
    {
        public static readonly Uri SalesForceTokenUrl = new Uri("https://login.salesforce.com/services/oauth2/token");
        public static readonly Uri SalesForceAuthUrl = new Uri("https://login.salesforce.com/services/oauth2/authorize");

        public const string IdToken = "id_token";
        public const string InstanceUrl = "instance_url";

        public SalesForceOAuthApi(OAuthConfig config) : base(config, SalesForceTokenUrl, SalesForceAuthUrl)
        {
        }
    }

}
