using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using Avend.API;
using Avend.API.Infrastructure;
using Avend.OAuth;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace Avend.ApiTests.Infrastructure
{
    public class TestSystem
    {
        public static readonly TestSystem Instance = new TestSystem();

        public TestSystem()
        {
            FixWorkingDirectory();

            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<TestStartup>();
            
            TestServer = new TestServer(webHostBuilder);

            Environment = (IHostingEnvironment)TestServer.Host.Services.GetService(typeof(IHostingEnvironment));            
            Configuration = (IConfiguration)TestServer.Host.Services.GetService(typeof(IConfiguration));
            FakeAD = (FakeActiveDirectory)TestServer.Host.Services.GetService(typeof(FakeActiveDirectory));

            TesterAppOauthConfig = new OAuthConfig
            {
                ClientId = "be23f8c9-80be-4e17-ab70-e48ce5939fdd",
                ClientSecret = "W8C4CHIpMKKMiNQS3isEBpMf4srg4MldyhtU9oywTkc=",
                ReturnUrl = "https://localhost:3000/user_created"
            };

            var usersManagement = Configuration.GetSection("Authentication").GetSection("UsersManagement");
            UsersManagementOAuthConfig = new OAuthConfig
            {
                ClientId = usersManagement.GetSection("ClientId").Value,
                ClientSecret = usersManagement.GetSection("ClientSecret").Value,
                ReturnUrl = usersManagement.GetSection("ReturnUrl").Value
            };

            var azureAd = Configuration.GetSection("Authentication").GetSection("AzureAD");
            TenantId = azureAd.GetSection("TenantUid").Value;
            ClientId = azureAd.GetSection("ClientId").Value;
            TenantName = azureAd.GetSection("TenantName").Value;

            AvendOAuthConfig = new OAuthConfig
            {
                ClientId = ClientId,
                ClientSecret = "sZsA7A62zGoRtY.%",
                ReturnUrl = "https://avend-dev-web.azurewebsites.net"
            };
        }

        public static void FixWorkingDirectory()
        {
//Current directory is different when running test from ReSharper and MSTest
            //ReShaprper set output directory to build output which break tests.
            //Below few lines fix this by resetting current dir to Avend.ApiTests
            //(averbin)
            var currentDir = Directory.GetCurrentDirectory();
            const string testDir = "Avend.ApiTests";
            var idx = currentDir.IndexOf(testDir, StringComparison.Ordinal);
            currentDir = currentDir.Substring(0, idx + testDir.Length);
            Directory.SetCurrentDirectory(currentDir);
        }

        public TestServer TestServer { get; }
        public OAuthConfig TesterAppOauthConfig { get; }
        public string TenantId { get; }
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public string TenantName { get; }
        public OAuthConfig UsersManagementOAuthConfig { get; }
        public string ClientId { get; }
        public OAuthConfig AvendOAuthConfig { get; }
        public FakeActiveDirectory FakeAD { get; }

        public GraphServiceClient GetMsGraphClient()
        {
            var azure = new AzureOAuthApi(UsersManagementOAuthConfig);
            azure.SetTenantId(TenantId);
            Dictionary<string, object> authResponse = null;
            var authProvider = new DelegateAuthenticationProvider(async m =>
            {
                if (authResponse == null)
                {
                    authResponse = await azure.LoginWithClientCredentials(new Dictionary<string, string>
                    {
                        {"resource", "https://graph.microsoft.com"}
                    });
                }
                m.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                    authResponse[OAuthApi.AccessToken] as string);
            });
            return new GraphServiceClient(authProvider);
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }

        public HttpClient CreateClient()
        {
            var httpClient = TestServer.CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress.AbsoluteUri + "api/v1/");
            return httpClient;
        }

        public HttpClient CreateClient(string token)
        {            
            var c = CreateClient();
            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return c;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class TestStartup : Startup
        {
            public TestStartup(IHostingEnvironment env) : base(env)
            {
            }

            public override IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(Configuration);
                services.AddSingleton(new FakeActiveDirectory());                
                return base.ConfigureServices(services);
            }

            protected override void AddEmail(IServiceCollection services)
            {
                services.Configure<SendgridConfiguration>(options => Configuration.GetSection("SendGrid").Bind(options));
                services.AddSingleton<ISendGrid, TestSendGrid>();
            }

            protected override void ConfigureAuthentication(IApplicationBuilder appBuilder)
            {
                //appBuilder.UseMiddleware<SubscriptionMemberDataLoader>();
                appBuilder.UseTestAuthentication();
            }
        }

        public IServiceScope GetServices()
        {
            var scopeFactory = TestServer.Host.Services.GetService<IServiceScopeFactory>();
            return scopeFactory.CreateScope();
        }
    }
}