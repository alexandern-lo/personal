using System;
using NLog;
using LoggerNLog = NLog.Logger;

namespace SL4N.NLog
{
	public class NLogLogger : ILogger
	{
		private LoggerNLog logger;

		public NLogLogger(LoggerNLog logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));
			this.logger = logger;
		}

		public bool IsErrorEnabled
		{
			get
			{
				return logger.IsErrorEnabled;
			}
		}

		public bool IsDebugEnabled
		{
			get
			{
				return logger.IsDebugEnabled;
			}
		}

		public bool IsInfoEnabled
		{
			get
			{
				return logger.IsInfoEnabled;
			}
		}

		public string Name
		{
			get
			{
				return logger.Name;
			}
		}

		public bool IsTraceEnabled
		{
			get
			{
				return logger.IsTraceEnabled;
			}
		}

		public bool IsWarnEnabled
		{
			get
			{
				return logger.IsWarnEnabled;
			}
		}

		public void Debug(string msg)
		{
			logger.Debug(msg);
		}

		public void Debug(string msg, Exception t)
		{
			logger.Debug(t, msg);
		}

		public void Debug(Marker marker, string msg)
		{
			throw new NotImplementedException();
		}

		public void Debug(string format, params object[] arguments)
		{
			logger.Debug(format, arguments);
		}

		public void Debug(string format, object arg)
		{
			logger.Debug(format, arg);
		}

		public void Debug(Marker marker, string format, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public void Debug(Marker marker, string format, object arg)
		{
			throw new NotImplementedException();
		}

		public void Debug(Marker marker, string msg, Exception t)
		{
			throw new NotImplementedException();
		}

		public void Debug(string format, object arg1, object arg2)
		{
			logger.Debug(format, arg1, arg2);
		}

		public void Debug(Marker marker, string format, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void Error(string msg)
		{
			logger.Error(msg);
		}

		public void Error(string format, params object[] arguments)
		{
			logger.Error(format, arguments);
		}

		public void Error(Marker marker, string msg)
		{
			throw new NotImplementedException();
		}

		public void Error(string msg, Exception t)
		{
			logger.Error(t, msg);
		}

		public void Error(string format, object arg)
		{
			logger.Error(format, arg);
		}

		public void Error(Marker marker, string format, object arg)
		{
			throw new NotImplementedException();
		}

		public void Error(Marker marker, string msg, Exception t)
		{
			throw new NotImplementedException();
		}

		public void Error(Marker marker, string format, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public void Error(string format, object arg1, object arg2)
		{
			logger.Error(format, arg1, arg2);
		}

		public void Error(Marker marker, string format, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void Info(string msg)
		{
			logger.Info(msg);
		}

		public void Info(string format, params object[] arguments)
		{
			logger.Info(format, arguments);
		}

		public void Info(Marker marker, string msg)
		{
			throw new NotImplementedException();
		}

		public void Info(string msg, Exception t)
		{
			logger.Info(t, msg);
		}

		public void Info(string format, object arg)
		{
			logger.Info(format, arg);
		}

		public void Info(Marker marker, string format, object arg)
		{
			throw new NotImplementedException();
		}

		public void Info(Marker marker, string msg, Exception t)
		{
			throw new NotImplementedException();
		}

		public void Info(Marker marker, string format, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public void Info(string format, object arg1, object arg2)
		{
			logger.Info(format, arg1, arg2);
		}

		public void Info(Marker marker, string format, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public bool GetIsErrorEnabled(Marker marker)
		{
			throw new NotImplementedException();
		}

		public bool GetIsTraceEnabled(Marker marker)
		{
			throw new NotImplementedException();
		}

		public bool GetIsWarnEnabled(Marker marker)
		{
			throw new NotImplementedException();
		}

		public void Trace(string msg)
		{
			logger.Trace(msg);
		}

		public void Trace(string format, params object[] arguments)
		{
			logger.Trace(format, arguments);
		}

		public void Trace(Marker marker, string msg)
		{
			throw new NotImplementedException();
		}

		public void Trace(string msg, Exception t)
		{
			logger.Trace(t, msg);
		}

		public void Trace(string format, object arg)
		{
			logger.Trace(format, arg);
		}

		public void Trace(Marker marker, string format, object arg)
		{
			throw new NotImplementedException();
		}

		public void Trace(Marker marker, string msg, Exception t)
		{
			throw new NotImplementedException();
		}

		public void Trace(Marker marker, string format, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public void Trace(string format, object arg1, object arg2)
		{
			logger.Trace(format, arg1, arg2);
		}

		public void Trace(Marker marker, string format, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public void Warn(string msg)
		{
			logger.Warn(msg);
		}

		public void Warn(string format, params object[] arguments)
		{
			logger.Warn(format, arguments);
		}

		public void Warn(Marker marker, string msg)
		{
			throw new NotImplementedException();
		}

		public void Warn(string msg, Exception t)
		{
			logger.Warn(t, msg);
		}

		public void Warn(string format, object arg)
		{
			logger.Warn(format, arg);
		}

		public void Warn(Marker marker, string format, object arg)
		{
			throw new NotImplementedException();
		}

		public void Warn(Marker marker, string msg, Exception t)
		{
			throw new NotImplementedException();
		}

		public void Warn(Marker marker, string format, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public void Warn(string format, object arg1, object arg2)
		{
			logger.Warn(format, arg1, arg2);
		}

		public void Warn(Marker marker, string format, object arg1, object arg2)
		{
			throw new NotImplementedException();
		}

		public bool GetIsDebugEnabled(Marker marker)
		{
			throw new NotImplementedException();
		}

		public bool GetIsInfoEnabled(Marker marker)
		{
			throw new NotImplementedException();
		}
	}
}

