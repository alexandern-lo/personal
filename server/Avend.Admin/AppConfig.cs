using Avend.OAuth;

namespace Avend.Admin
{
    public class AppConfig
    {
        public string TenantName { get; set; }
        public string TenantUid { get; set; }
        public OAuthConfig UsersManagement { get; set; }
    }
}