using Microsoft.Extensions.Logging;

namespace Avend.API.Infrastructure.Logging
{
    public static class LoggingEvents
    {
        public static readonly EventId STARTUP = new EventId(100, "STARTUP");
        public static readonly EventId LEAD_EXPORT = new EventId(1000, "LEAD_EXPORT");
        public static readonly EventId SUBSCRIPTION = new EventId(2000, "SUBSCRIPTION");
    }
}
