using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SL4N;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

#if __IOS__
using Foundation;
#endif

#if __ANDROID__
using Android.Webkit;
using Android.App;
using Android.OS;

[assembly: UsesPermission(Name = "android.permission.INTERNET")]
#endif


namespace LiveOakApp.Models.Services
{
    public class AuthService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<AuthService>();

        public User CurrentUser { get; private set; }
        public string GraphToken { get; private set; }
        string TenantId { get; set; }

        Microsoft.Identity.Client.User AuthUser { get; set; }

        public string AuthToken { get; set; }

        DateTimeOffset AuthTokenExpiresOn { get; set; }

        readonly CacheStorage CacheStorage;
        readonly UserSQLiteDatabase UserSQLiteDatabase;

        public AuthService(CacheStorage cacheStorage, UserSQLiteDatabase userSQLiteDatabase)
        {
            LOG.Info("MobileServiceClient created");
            CacheStorage = cacheStorage;
            UserSQLiteDatabase = userSQLiteDatabase;
            SetupAuthService();
        }

        void SetupAuthService()
        {
            ClientApplication = new PublicClientApplication(ClientID);

            try
            {
                var savedAppItems = ClientApplication.UserTokenCache.ReadItems(ClientID);
                if (savedAppItems.Any())
                {
                    var savedAppToken = savedAppItems.FirstOrDefault(tokenData => tokenData.Scope.Contains(ClientID));
                    var savedGraphToken = savedAppItems.FirstOrDefault(tokenData => tokenData.Scope.Contains(GraphScopes.First()));

                    if (savedAppToken != null)
                    {
                        TenantId = savedAppToken.TenantId;
                        AuthUser = savedAppToken.User;
                        AuthTokenExpiresOn = savedAppToken.ExpiresOn;

                        if (AuthTokenExpiresOn < DateTimeOffset.Now)
                        {
                            LOG.Warn("auth token expired ({0} < {1})", AuthTokenExpiresOn.ToLocalTime(), DateTimeOffset.Now);
                        }
                        AuthToken = savedAppToken.Token;

                        CurrentUser = new User(savedAppToken);
                        UserSQLiteDatabase.UseDatabaseForUserId(AuthUser.UniqueId);
                    }

                    if (savedGraphToken != null)
                    {
                        GraphToken = savedGraphToken.Token;
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("SetupAuthService failure", ex);
                Clear();
            }
        }

        public async Task<User> Login(IPlatformParameters platformParameters)
        {
            ClientApplication.PlatformParameters = platformParameters;

            var result = await ClientApplication.AcquireTokenAsync(Scopes, string.Empty, UiOptions.SelectAccount, string.Empty, null, Authority, SignUpSignInpolicy);

            return await SetupWithResult(result);
        }

        public async Task<User> ResetPassword(IPlatformParameters platformParameters)
        {
            ClientApplication.PlatformParameters = platformParameters;

            var result = await ClientApplication.AcquireTokenAsync(Scopes, string.Empty, UiOptions.SelectAccount, string.Empty, null, Authority, ResetPasswordPolicy);

            return await SetupWithResult(result);
        }

        public async Task<User> SilentLogin(IPlatformParameters platformParameters = null)
        {
            platformParameters = platformParameters ?? NavigationManager.Instance.CurrentPlatformParameters();
            if (platformParameters != null)
            {
                ClientApplication.PlatformParameters = platformParameters;
            }

            AuthenticationResult result;
            try
            {
                result = await ClientApplication.AcquireTokenSilentAsync(Scopes, string.Empty, Authority, SignUpSignInpolicy, false);
            }
            catch (MsalSilentTokenAcquisitionException tokenError)
            {
                MsalServiceException error = tokenError.InnerException as MsalServiceException;
                if (error?.ErrorCode == "invalid_grant")
                {
                    await Logout();
                    NavigationManager.Instance.NavigateToRequiredStateIfNeeded();
                }
                throw;
            }

            return await SetupWithResult(result);
        }

        async Task<User> SetupWithResult(AuthenticationResult result)
        {
            AuthUser = result.User;
            TenantId = result.TenantId;
            AuthToken = result.Token;
            AuthTokenExpiresOn = result.ExpiresOn;

            CurrentUser = new User(result);
            UserSQLiteDatabase.UseDatabaseForUserId(AuthUser.UniqueId);

            await CacheStorage.DropAllObjectsIfUserDiffers(AuthUser.UniqueId);

            // TODO: GraphApi Karyaev
            //GraphToken = await RequestGraphAccessToken();

            ServiceLocator.Instance.OnLoggedInSetup();

            return CurrentUser;
        }

        async Task<string> RequestGraphAccessToken()
        {
            var token = "";
            try
            {
                var result = await ClientApplication.AcquireTokenAsync(GraphScopes, AuthUser, UiOptions.ForceConsent, String.Empty, null, "https://login.microsoftonline.com/common/", "");
                //  var result = await ClientApplication.AcquireTokenSilentAsync(AuthScopes, AuthUser, "https://login.microsoftonline.com/common/", SignUpSignInpolicy, false);
                //  var result = await ClientApplication.AcquireTokenSilentAsync(AuthScopes, AuthUser, Authority, SignUpSignInpolicy, false);

                token = result.Token;

                //get data from API
                HttpClient client = ServiceLocator.Instance.ApiService.CreateHttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    LOG.Error("Something went wrong with the API call: {0}", responseString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return token;
        }

        public async Task Logout()
        {
            Clear();
            await ServiceLocator.Instance.OnLogoutCleanup();

            UserSQLiteDatabase.UseDatabaseForUserId(null);

#if __ANDROID__
            var cookieManager = CookieManager.Instance;
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                cookieManager.RemoveAllCookies(null);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete
                CookieSyncManager.CreateInstance(Application.Context);
#pragma warning restore CS0618 // Type or member is obsolete
                cookieManager.RemoveAllCookie();
            }
#endif

#if __IOS__
            var cookieStorage = NSHttpCookieStorage.SharedStorage;
            foreach (NSHttpCookie cookie in cookieStorage.Cookies)
            {
                cookieStorage.DeleteCookie(cookie);
            }
#endif
        }

        void Clear()
        {
            TenantId = null;
            AuthUser = null;
            CurrentUser = null;
            GraphToken = null;
            foreach (var user in ClientApplication.Users)
            {
                user.SignOut();
            }
            ClientApplication.UserTokenCache.Clear(ClientID);
        }

        bool HasToken
        {
            get
            {
                return AuthToken != null;
            }
        }

        public bool HasCachedTokens
        {
            get
            {
                return ClientApplication.UserTokenCache.Count > 0;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return CurrentUser != null && HasToken;
            }
        }

        public bool IsAuthTokenExpired
        {
            get
            {
                return AuthTokenExpiresOn < DateTimeOffset.Now;
            }
        }

        static string ClientID = ApplicationConfig.AuthClientID;
        static string[] Scopes = ApplicationConfig.AuthScopes;
        static string[] GraphScopes = { "user.read" };
        static string SignUpSignInpolicy = ApplicationConfig.AuthPolicy;
        static string ResetPasswordPolicy = ApplicationConfig.AuthResetPasswordPolicy;
        static string Authority = "https://login.microsoftonline.com/" + ApplicationConfig.AuthAuthorityAD + "/";

        static PublicClientApplication ClientApplication { get; set; }
    }
}
