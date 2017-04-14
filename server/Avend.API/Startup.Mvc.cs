using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Avend.API
{
    public partial class Startup
    {
        private readonly string policyName = "CorsAllowAll";

        protected virtual IMvcCoreBuilder AddMvcServices(IServiceCollection services)
        {
            var mvcServices = services.AddMvcCore();
            mvcServices.AddJsonFormatters(
                    options =>
                    {
                        options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.Formatting = Formatting.Indented;
                        options.NullValueHandling = NullValueHandling.Ignore;                        
                    })
                .AddXmlSerializerFormatters()                
                .AddCors(options =>
                {
                    options.AddPolicy(policyName, builder => builder
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowCredentials()
                    );
                })                
                .AddApiExplorer();

            services.Configure<MvcOptions>(
                options => options.Filters.Add(new CorsAuthorizationFilterFactory(policyName)));
            return mvcServices;
        }

        protected virtual void ConfigureMvc(IApplicationBuilder appBuilder)
        {
            appBuilder.UseMvc();
        }
    }
}
