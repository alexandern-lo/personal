using System;
using System.Linq;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.API.Services;
using Avend.API.Services.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void AddDatabaseServices(IServiceCollection services)
        {
            services
                .AddSingleton(new AppSettings
                {
                    DatabaseConnectionString = Configuration.GetSection("Data:SQL:ConnectionString").Value,
                    StorageConnectionString = Configuration.GetSection("Data:Storage:ConnectionString").Value,
                })
                .AddDbContext<AvendDbContext>((serviceProvider, options) =>
                {
                    options.UseSqlServer(
                        Configuration.GetSection("Data:SQL:ConnectionString").Value
                    );
                    options.UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
                });
        }

        protected virtual void ConfigureDatabase(IApplicationBuilder appBuilder)
        {
            try
            {
                var serviceScopeFactory = appBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var avendDbContext = scope.ServiceProvider.GetService<AvendDbContext>();
                    _logger.LogInformation("Database migration started");
                    avendDbContext.Database.Migrate();
                    EnsureDbHasRequiredData(avendDbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(LoggingEvents.STARTUP, "Cannot setup database: " + ex.Message);

                throw;
            }
            _logger.LogDebug(LoggingEvents.STARTUP, "Database setup completed");
        }

        public static void EnsureDbHasRequiredData(AvendDbContext usersDb)
        {
            var terms = (from t in usersDb.TermsTable select t).Take(1).ToList();
            if (terms.Count == 0)
            {
                var defaultTerms = new Terms
                {
                    Uid = Guid.NewGuid(),
                    TermsText = "Default terms and conditions text. Specify it usign admin panel.",
                    CreatedAt = DateTime.Now,
                    ReleaseDate = DateTime.Now
                };
                usersDb.TermsTable.Add(defaultTerms);
                usersDb.SaveChanges();
            }

            var crmSystems = (from s in usersDb.CrmSystemsTable select s).Take(1).ToList();
            if (crmSystems.Count == 0)
            {
                foreach (var abbreviation in CrmDefaultsHelper.AvailableCrmSystemAbbreviations)
                {
                    if (usersDb.CrmSystemsTable.Any(record => record.Abbreviation == abbreviation))
                        continue;

                    var crm = new CrmSystem
                    {
                        Uid = Guid.NewGuid(),
                        Abbreviation = abbreviation,
                        Name = CrmDefaultsHelper.CrmNames[abbreviation],
                        DefaultFieldMappings =
                            JsonConvert.SerializeObject(CrmDefaultsHelper.DefaultCrmMappings[abbreviation],
                                Formatting.Indented),
                        AuthorizationParams =
                            JsonConvert.SerializeObject(CrmDefaultsHelper.CrmAuthorizationParams[abbreviation]),
                        TokenRequestUrl = CrmDefaultsHelper.CrmTokenRequestUrls[abbreviation],
                        TokenRequestParams =
                            JsonConvert.SerializeObject(CrmDefaultsHelper.CrmTokenRequestParams[abbreviation]),
                    };

                    usersDb.CrmSystemsTable.Add(crm);
                }

                usersDb.SaveChanges();
            }
        }
    }
}