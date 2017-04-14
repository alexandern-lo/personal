using System;
using System.Collections.Concurrent;

namespace SL4N
{
	public abstract class LoggerFactoryBase : ILoggerFactory
	{
		ConcurrentDictionary<string, ILogger> loggerMap = new ConcurrentDictionary<string, ILogger>();

		public ILogger GetLogger(string name)
		{
			ILogger slf4jLogger;
			if (loggerMap.TryGetValue(name, out slf4jLogger))
				return slf4jLogger;
			else {
				ILogger newLogger = CreateNewLogger(name);
				return loggerMap.AddOrUpdate(name, newLogger, (key, logger) => logger);
			}
		}

		protected abstract ILogger CreateNewLogger(string name);

	}
}

