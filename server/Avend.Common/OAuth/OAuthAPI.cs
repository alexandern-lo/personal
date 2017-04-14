using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Qoden.Validation;

namespace Avend.OAuth
{
    public class OAuthApi
    {
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
        public const string TokenType = "token_type";
        public const string ExpiresIn = "expires_in";
        public const string Error = "error";
        public const string ErrorMessage = "error_message";

        public OAuthApi(OAuthConfig config, Uri tokenUrl, Uri authUrl)
        {
            config.Validate();
            Config = config;
            TokenUrl = tokenUrl;
            AuthUrl = authUrl;
        }

        public OAuthConfig Config { get; }
        private Uri _tokenUrl;
        public Uri TokenUrl {
            get { return _tokenUrl; }
            set
            {
                Assert.Argument(value, "TokenUrl").NotNull().IsAbsoluteUri();
                _tokenUrl = value;
            }
        }
        private Uri _authUrl;
        public Uri AuthUrl {
            get { return _authUrl;  }
            set
            {
                Assert.Argument(value, "AuthUrl").NotNull().IsAbsoluteUri();
                _authUrl = value;
            }
        }

        /// <summary>
        /// Generate authorization page url.
        /// </summary>
        /// <param name="requestParameters">Query parmaetesr to be added in addition to default parameters</param>
        /// <returns></returns>
        public virtual Uri GetAuthorizationPageUrl(Dictionary<string, string> requestParameters = null)
        {
            if (requestParameters == null) requestParameters = new Dictionary<string, string>();
            AddRequestParameter(ref requestParameters, "client_id", Config.ClientId);
            AddRequestParameter(ref requestParameters, "redirect_uri", Config.ReturnUrl);
            AddRequestParameter(ref requestParameters, "base", AuthUrl.AbsoluteUri);
            AddRequestParameter(ref requestParameters, "response_type", "code");
            AddRequestParameter(ref requestParameters, "response_mode", "query");

            return AuthRequestUrl(AuthUrl, requestParameters);
        }

        public virtual async Task<Dictionary<string, object>> LoginWithUsernamePassword(string username, string password, Dictionary<string, string> requestParameters = null)
        {
            Assert.Argument(username, "username").NotEmpty();
            Assert.Argument(password, "password").NotNull();
            AddRequestParameter(ref requestParameters, "username", username);
            AddRequestParameter(ref requestParameters, "password", password);
            AddRequestParameter(ref requestParameters, "grant_type", "password");
            return await LoginWithClientCredentials(requestParameters);
        }

        /// <summary>
        /// Login with client id and secret specified in Config
        /// </summary>
        /// <param name="requestParameters">additional request parameters</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> LoginWithClientCredentials(Dictionary<string, string> requestParameters = null)
        {
            AddRequestParameter(ref requestParameters, "grant_type", "client_credentials");
            return await Login(requestParameters);
        }

        /// <summary>
        /// Exchange grant code to access and refresh token.
        /// </summary>
        /// <param name="grantCode">grant code</param>
        /// <param name="requestParameters">additional request parameters</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> LoginWithGrantCode(string grantCode, Dictionary<string, string> requestParameters = null)
        {
            Assert.Argument(grantCode, "grantCode").NotEmpty();
            AddRequestParameter(ref requestParameters, "code", grantCode);
            AddRequestParameter(ref requestParameters, "grant_type", "authorization_code");
            return await Login(requestParameters);
        }

        /// <summary>
        /// Exchange refresh token from userCrmConfiguration to access and refresh tokens. Method will fail if CRM configuration does not have refresh token.
        /// </summary>
        /// <param name="refreshToken">refresh token</param>
        /// <param name="requestParameters">additional request parameters</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> LoginWithRefreshToken(string refreshToken, Dictionary<string, string> requestParameters = null)
        {
            Assert.Argument(refreshToken, "refreshToken").NotEmpty();
            AddRequestParameter(ref requestParameters, "grant_type", "refresh_token");
            AddRequestParameter(ref requestParameters, "refresh_token", refreshToken);
            return await Login(requestParameters);
        }

        /// <summary>
        /// Most generic login method. Client expected to specify app parmaetesr excep client_id, client_secret and redirect_url
        /// </summary>
        /// <param name="requestParameters">Login request parameters</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, object>> Login(Dictionary<string, string> requestParameters)
        {
            AddRequestParameter(ref requestParameters, "client_id", Config.ClientId);
            AddRequestParameter(ref requestParameters, "client_secret", Config.ClientSecret);
            AddRequestParameter(ref requestParameters, "redirect_uri", Config.ReturnUrl);

            var body = new FormUrlEncodedContent(requestParameters);
            using (var http = new HttpClient())
            {
                var responseMessage = await http.PostAsync(TokenUrl, body);
                return await ProcessResponseMessage(responseMessage);
            }
        }


        private static async Task<Dictionary<string, object>> ProcessResponseMessage(HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (!responseMessage.IsSuccessStatusCode)
                throw new OAuthException(responseMessage, response);
            return response;
        }

        protected static void AddRequestParameter(ref Dictionary<string, string> context, string paramName, string paramValue)
        {
            if (context == null) context = new Dictionary<string, string>();
            if (!context.ContainsKey(paramName)) context.Add(paramName, paramValue);
        }

        private Uri AuthRequestUrl(Uri baseUri, Dictionary<string, string> requestParameters)
        {
            var url = new StringBuilder(baseUri.AbsoluteUri);
            url.Append("?");

            var kvs = requestParameters.ToList();
            for (int i = 0; i < kvs.Count; ++i)
            {
                if (i > 0)
                    url.Append('&');

                if (kvs[i].Value != null)
                    url.Append(kvs[i].Key).Append('=').Append(UrlEncoder.Default.Encode(kvs[i].Value));
            }

            return new Uri(url.ToString());
        }
    }

    public static class OAuthConfigExtensions
    {
        public static void Validate(this OAuthConfig config)
        {
            Validate(config, ArgumentValidator.Instance);
        }

        public static void Validate(this OAuthConfig config, IValidator errors)
        {
            errors.CheckValue(config, "config").NotNull();
            errors.CheckValue(config.ClientId, "clientId").NotEmpty();
            errors.CheckValue(config.ClientSecret, "clientSecret").NotEmpty();
        }
    }

    /// <summary>
    /// A container for OAuth client configuration.
    /// </summary>
    public class OAuthConfig
    {
        /// <summary>
        /// Client ID, usually generated usign Oauth server developer portal.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client Secret usually generated with OAuth server developer portal.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// URL to redirect to after successfull authentication.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Root Url for the API, could have different 3rd-level subdomains.
        /// </summary>
        public string ApiRootUrl { get; set; }
    }
    
    public class OAuthException : Exception
    {
        public OAuthException(HttpResponseMessage responseMessage, Dictionary<string, object> response) : base(ErrorMessage(responseMessage, response))
        {
            Response = response;
            ResponseMessage = responseMessage;
        }

        public Dictionary<string, object> Response { get; private set; }
        public HttpResponseMessage ResponseMessage { get; private set; }

        private static string ErrorMessage(HttpResponseMessage responseMessage, Dictionary<string, object> response)
        {
            if (response != null && response.ContainsKey("error"))
            {
                if (response.ContainsKey("error_description"))
                    return String.Format("{0}: {1}", response["error"], response["error_description"]);
                else
                    return String.Format("{0}", response["error"]);
            }
            else
            {
                return String.Format("{0}: {1}", responseMessage.StatusCode, responseMessage.ReasonPhrase);
            }
        }
    }
}
