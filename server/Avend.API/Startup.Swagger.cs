using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void AddSwaggerServices(IServiceCollection services)
        {            
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Avend API",
                    Description = "API for the Avend CMS",
                    TermsOfService = "<Please add Terms of Service>",
                    Contact = new Contact
                    {
                        Name = "Konstantin Karyaev",
                        Email = "support@avend.co",
                        Url = "http://avend.co"
                    },
                    License = new License
                    {
                        Name = "<add license here>",
                        Url = "http://avend.co"
                    },
                });

                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(basePath, "Avend.API.xml");
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });
        }

        protected virtual void ConfigureSwagger(IApplicationBuilder appBuilder)
        {
            appBuilder.UseSwaggerUi();
            appBuilder.UseSwagger();
        }
    }
}
