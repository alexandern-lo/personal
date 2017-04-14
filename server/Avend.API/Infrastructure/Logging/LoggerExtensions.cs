using System;
using Microsoft.Extensions.Logging;

namespace Avend.API.Infrastructure.Logging
{
    public static class LoggerExtensions
    {
        public static void LogTrace(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogTrace(0, e, message, p);
        }

        public static void LogDebug(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogDebug(0, e, message, p);
        }

        public static void LogInformation(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogInformation(0, e, message, p);
        }

        public static void LogWarning(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogWarning(0, e, message, p);
        }

        public static void LogError(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogError(0, e, message, p);
        }

        public static void LogCritical(this ILogger logger, Exception e, string message, params object[] p)
        {
            logger.LogCritical(0, e, message, p);
        }


    }
}
