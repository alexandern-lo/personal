namespace LiveOakApp.Models.Services
{
    public static class ApplicationConfig
    {
#if PRODUCTION
        public const string ApiRootUrl = "https://api.avend.co/";
        public const string WebLoginUrl = "https://portal.avend.co/";
        public const string SubscriptionRenewUrl = "https://portal.avend.co/";

        public const string AuthClientID = "39d6697d-7baa-48b3-a6eb-1dd8c2c83bbe";
        public const string AuthPolicy = "b2c_1_sign_in";
        public const string AuthResetPasswordPolicy = "b2c_1_reset_pass";
        public const string AuthAuthorityAD = "avend.onmicrosoft.com";
        public static string[] AuthScopes = { AuthClientID };

        public const string BuildSuffix = "-production";
#else
        public const string ApiRootUrl = "https://avend-stage-api.azurewebsites.net/";
        public const string WebLoginUrl = "https://avend-stage-web.azurewebsites.net/";
        public const string SubscriptionRenewUrl = "https://avend-stage-web.azurewebsites.net/";

        public const string AuthClientID = "c6673a37-206d-49dc-ab56-2050057c1cca";
        public const string AuthPolicy = "b2c_1_sign_in";
        public const string AuthResetPasswordPolicy = "b2c_1_pass_reset";
        public const string AuthAuthorityAD = "avendstage.onmicrosoft.com";
        public static string[] AuthScopes = { AuthClientID };

        public const string BuildSuffix = "-staging";
#endif
    }
}
