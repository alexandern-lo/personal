using System;

using Avend.API.Services;
using Avend.API.BL;
using Avend.API.Controllers.v1;
using Avend.API.Middleware;
using Avend.API.Model;
using Avend.API.Services.Crm;
using Avend.API.Services.Dashboard;
using Avend.API.Services.Events;
using Avend.API.Services.Leads;
using Avend.API.Services.Resources;
using Avend.API.Services.Subscriptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Avend.API
{
    public partial class Startup
    {
        private ILogger _logger;

        public Startup(IHostingEnvironment env)
        {
            var localEnvVar = Environment.GetEnvironmentVariable("LOCAL");
            Console.WriteLine($"LOCAL = {localEnvVar}");
            Console.WriteLine(JsonConvert.SerializeObject(env));
            Configuration = GetConfig(env);
        }

        protected static IConfiguration GetConfig(IHostingEnvironment env)
        {
            var root = env.ContentRootPath;
            var builder = new ConfigurationBuilder()
                .SetBasePath(root)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            if ("1" == Environment.GetEnvironmentVariable("LOCAL"))
            {
                builder.AddJsonFile("appsettings.local.json", true);
            }

            return builder.Build();
        }

        public IConfiguration Configuration { get; }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            var mvc = AddMvcServices(services);

            AddAuthServices(services, mvc);

            AddSwaggerServices(services);

            AddAppSettings(services);

            AddEmail(services);

            services
                .AddApplicationInsightsTelemetry(Configuration)                
                .AddSingleton<CrmConnectorFactory>()
                .AddScoped<RecurlyService>()
                .AddSingleton<UserCrmDtoBuilder>()
                .AddScoped<ProfileService>()
                .AddScoped<UsersManagementService>()
                .AddScoped<EventUserExpensesWriter>()
                .AddScoped<InviteService>()
                .AddScoped<SubscriptionsService>()
                .AddScoped<DashboardService>()
                .AddScoped<SuperadminDashboardService>()
                .AddScoped<EventsService>()
                .AddScoped<AgendaItemsService>()
                .AddScoped<EventUserGoalsService>()
                .AddScoped<EventUserExpensesService>()
                .AddScoped<LeadsCrudService>()
                .AddScoped<LeadsExportToFileService>()
                .AddScoped<LeadsExportToCrmService>()
                .AddScoped<QuestionsService>()
                .AddScoped<ResourcesService>()
                .AddScoped<AttendeesService>()
                .AddScoped<AttendeeCategoriesService>()
                .AddScoped<CrmConfigurationService>()
                .AddScoped(sp =>
                {
                    var httpAccessor = sp.GetService<IHttpContextAccessor>();
                    var ctx = httpAccessor.HttpContext.GetUserContext() ?? new UserContext(sp.GetService<DbContextOptions<AvendDbContext>>());
                    return ctx;
                });

            AddDatabaseServices(services);

            return services.BuildServiceProvider();
        }

        public virtual void Configure(IApplicationBuilder appBuilder, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            ConfigureLogging(appBuilder, loggerFactory);

            _logger = loggerFactory.CreateLogger<Startup>();

            ConfigureTiming(appBuilder);

            appBuilder.UseDeveloperExceptionPage();

            ConfigureErrors(appBuilder);

            ConfigureDatabase(appBuilder);

            ConfigureAuthentication(appBuilder);

            ConfigureSwagger(appBuilder);

            appBuilder.UseMiddleware<ApiVersionMiddleware>();

            ConfigureMvc(appBuilder);
        }
    }
}
