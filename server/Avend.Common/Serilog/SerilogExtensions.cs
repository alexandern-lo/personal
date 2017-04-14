using Microsoft.Extensions.Logging;

namespace Avend.API.Infrastructure
{
    public static class SerilogExtensions
    {
        public static ILoggerFactory AddSerilogProvider(this ILoggerFactory factory, Serilog.ILogger logger = null)
        {
            factory.AddProvider(new SerilogLoggerProvider(logger));
            return factory;
        }
    }
}