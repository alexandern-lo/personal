using Avend.API.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void AddEmail(IServiceCollection services)
        {
            services.Configure<SendgridConfiguration>(options => Configuration.GetSection("SendGrid").Bind(options));
            services.AddScoped<ISendGrid, SendGrid>();
        }
    }
}
