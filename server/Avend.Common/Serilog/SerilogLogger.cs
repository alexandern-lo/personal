using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Serilog.Events;

namespace Avend.API.Infrastructure
{
    public class SerilogLogger : ILogger, ILogEventEnricher
    {
        private readonly Serilog.ILogger _logger;
        //ms logger store provided message in this property
        public const string OriginalFormatPropertyName = "{OriginalFormat}";

        public SerilogLogger(Serilog.ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _logger = logger.ForContext(this);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var level = ConvertLevel(logLevel);
            if (!_logger.IsEnabled(level))
            {
                return;
            }
            string messageFormat;
            List<object> props;
            ParseState(state as IEnumerable<KeyValuePair<string, object>>, out messageFormat, out props);
            if (messageFormat == null)
                messageFormat = formatter(state, exception);
            IEnumerable<LogEventProperty> boundProps;
            MessageTemplate messageTemplate;
            _logger.BindMessageTemplate(messageFormat, props.ToArray(), out messageTemplate, out boundProps);
            var properties = new List<LogEventProperty>
            {
                new LogEventProperty("EventId", new ScalarValue(eventId.Id)),
                new LogEventProperty("EventName", new ScalarValue(eventId.Name)),
            };
            properties.AddRange(boundProps);
            var logEvent = new LogEvent(DateTimeOffset.Now, level, exception, messageTemplate, properties);
            _logger.Write(logEvent);
        }

        private void ParseState(IEnumerable<KeyValuePair<string, object>> state, out string messageTemplate,
            out List<object> props)
        {
            messageTemplate = null;
            props = new List<object>();
            if (state == null) return;
            foreach (var kv in state)
            {
                if (kv.Key == OriginalFormatPropertyName && kv.Value is string)
                {
                    messageTemplate = (string) kv.Value;
                }
                else
                {
                    props.Add(kv.Value);
                }
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(ConvertLevel(logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return SerilogScope.Push(state);
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            SerilogScope.Current?.Enrich(logEvent, propertyFactory);
        }

        private static LogEventLevel ConvertLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return LogEventLevel.Fatal;
                case LogLevel.Error:
                    return LogEventLevel.Error;
                case LogLevel.Warning:
                    return LogEventLevel.Warning;
                case LogLevel.Information:
                    return LogEventLevel.Information;
                case LogLevel.Debug:
                    return LogEventLevel.Debug;
                // ReSharper disable once RedundantCaseLabel
                case LogLevel.Trace:
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}