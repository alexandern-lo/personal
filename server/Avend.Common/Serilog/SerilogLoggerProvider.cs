using System;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Avend.API.Infrastructure
{
    public class SerilogLoggerProvider : ILoggerProvider
    {
        private readonly Serilog.ILogger _logger;
        private readonly bool _dispose;

        public SerilogLoggerProvider(Serilog.ILogger logger = null, bool dispose = false)
        {
            _logger = logger ?? Log.Logger;
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (_dispose)
            {
                if (_logger != null)
                    (_logger as IDisposable)?.Dispose();
                else
                    Log.CloseAndFlush();
            }
        }

        public ILogger CreateLogger(string name)
        {
            return new SerilogLogger(_logger.ForContext("SourceContext", name));
        }
    }
}