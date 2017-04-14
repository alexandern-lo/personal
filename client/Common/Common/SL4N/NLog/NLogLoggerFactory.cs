using System;
using SL4N;
using NLog;

namespace SL4N.NLog
{
	public class NLogLoggerFactory : LoggerFactoryBase
	{
		protected override ILogger CreateNewLogger(string name)
		{
			return new NLogLogger(LogManager.GetLogger(name));
		}
	}
}

