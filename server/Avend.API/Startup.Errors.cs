using Avend.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void ConfigureErrors(IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ExceptionConverterMiddleware>();
        }
    }
}
