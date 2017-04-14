using Microsoft.Extensions.Logging;

namespace Avend.API.Infrastructure.Logging
{
    public static class AvendLog
    {
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();

        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);
    }
}
