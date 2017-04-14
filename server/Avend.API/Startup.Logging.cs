using Avend.API.Infrastructure;
using Avend.API.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void ConfigureLogging(IApplicationBuilder appBuilder, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransforming<EventId>(e => e.Name)
                .ReadFrom.Configuration(Configuration)                
                .CreateLogger();

            AvendLog.LoggerFactory = loggerFactory;
//            loggerFactory.AddSerilog();
            loggerFactory.AddSerilogProvider();

            //appBuilder.UseMiddleware<SerilogRequestMiddleware>();
        }
    }
}