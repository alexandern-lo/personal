using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avend.API.Infrastructure.Logging;
using Avend.API.Model;
using Avend.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Recurly.AspNetCore.Configuration;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void AddAppSettings(IServiceCollection services)
        {
            var appSettingsCrm = new AppSettingsCrm();
            appSettingsCrm.CrmConfigs[CrmSystemAbbreviation.Salesforce] =
                ReadOAuthConfig(Configuration.GetSection("Authentication:SalesForce"));
            appSettingsCrm.CrmConfigs[CrmSystemAbbreviation.Dynamics365] =
                ReadOAuthConfig(Configuration.GetSection("Authentication:Dynamics365"));

            if (_logger != null)
                _logger.LogDebug(LoggingEvents.STARTUP,
                    "AppSettingsCrm setup as\n" + JsonConvert.SerializeObject(appSettingsCrm));
            else
                Console.WriteLine("AppSettingsCrm setup as: " + JsonConvert.SerializeObject(appSettingsCrm));
            services.AddSingleton(appSettingsCrm);

            var recurlySection = Configuration.GetSection("Recurly");
            RecurlySection.Current = new RecurlySection(recurlySection);
        }

        public static OAuthConfig ReadOAuthConfig(IConfiguration section)
        {
            return new OAuthConfig
            {
                ClientId = section.GetSection("ClientId").Value,
                ClientSecret = section.GetSection("ClientSecret").Value,
                ReturnUrl = section.GetSection("ReturnURL").Value,
            };
        }
    }
}
