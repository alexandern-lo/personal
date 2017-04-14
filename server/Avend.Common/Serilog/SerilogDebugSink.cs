using System;
using System.Diagnostics;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Avend.API.Infrastructure
{
    class SerilogDebugSink : ILogEventSink
    {
        readonly ITextFormatter _textFormatter;

        public SerilogDebugSink(ITextFormatter textFormatter)
        {
            if (textFormatter == null) throw new ArgumentNullException(nameof(textFormatter));
            _textFormatter = textFormatter;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            var sr = new StringWriter();
            _textFormatter.Format(logEvent, sr);

            var text = sr.ToString().Trim();
            Debug.WriteLine(text);
        }
    }
}