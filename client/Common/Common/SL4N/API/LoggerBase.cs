using System;
namespace SL4N
{
	public abstract class LoggerBase : ILogger
	{
		public abstract string Name { get; }
		public abstract bool IsTraceEnabled { get; }
		public abstract bool IsDebugEnabled { get; }
		public abstract bool IsInfoEnabled { get; }
		public abstract bool IsWarnEnabled { get; }
		public abstract bool IsErrorEnabled { get; }

		public abstract void Debug(string msg);

		public abstract void Debug(string format, params object[] arguments);

		public abstract void Debug(string format, object arg);

		public abstract void Debug(string msg, Exception t);

		public abstract void Debug(string format, object arg1, object arg2);

		public void Debug(Marker marker, string msg)
		{
			Debug(msg);
		}

		public void Debug(Marker marker, string format, params object[] arguments)
		{
			Debug(format, arguments);
		}

		public void Debug(Marker marker, string format, object arg)
		{
			Debug(format, arg);
		}

		public void Debug(Marker marker, string msg, Exception t)
		{
			Debug(msg, t);
		}

		public void Debug(Marker marker, string format, object arg1, object arg2)
		{
			Debug(format, arg1, arg2);
		}

		public abstract void Error(string msg);
		public abstract void Error(string format, params object[] arguments);
		public abstract void Error(string format, object arg);
		public abstract void Error(string msg, Exception t);
		public abstract void Error(string format, object arg1, object arg2);

		public void Error(Marker marker, string msg)
		{
			Error(msg);
		}

		public void Error(Marker marker, string format, params object[] arguments)
		{
			Error(format, arguments);
		}

		public void Error(Marker marker, string format, object arg)
		{
			Error(format, arg);
		}

		public void Error(Marker marker, string msg, Exception t)
		{
			Error(msg, t);
		}

		public void Error(Marker marker, string format, object arg1, object arg2)
		{
			Error(format, arg1, arg2);
		}

		public abstract void Info(string msg);
		public abstract void Info(string format, params object[] arguments);
		public abstract void Info(string format, object arg);
		public abstract void Info(string msg, Exception t);
		public abstract void Info(string format, object arg1, object arg2);

		public void Info(Marker marker, string msg)
		{
			Info(msg);
		}

		public void Info(Marker marker, string format, params object[] arguments)
		{
			Info(format, arguments);
		}

		public void Info(Marker marker, string format, object arg)
		{
			Info(format, arg);
		}

		public void Info(Marker marker, string msg, Exception t)
		{
			Info(msg, t);
		}

		public void Info(Marker marker, string format, object arg1, object arg2)
		{
			Info(format, arg1, arg2);
		}

		public abstract void Trace(string msg);
		public abstract void Trace(string format, params object[] arguments);
		public abstract void Trace(string format, object arg);
		public abstract void Trace(string msg, Exception t);
		public abstract void Trace(string format, object arg1, object arg2);

		public void Trace(Marker marker, string msg)
		{
			Trace(msg);
		}

		public void Trace(Marker marker, string format, params object[] arguments)
		{
			Trace(format, arguments);
		}

		public void Trace(Marker marker, string format, object arg)
		{
			Trace(format, arg);
		}

		public void Trace(Marker marker, string msg, Exception t)
		{
			Trace(msg, t);
		}

		public void Trace(Marker marker, string format, object arg1, object arg2)
		{
			Trace(format, arg1, arg2);
		}

		public abstract void Warn(string msg);
		public abstract void Warn(string format, params object[] arguments);
		public abstract void Warn(string format, object arg);
		public abstract void Warn(string msg, Exception t);
		public abstract void Warn(string format, object arg1, object arg2);

		public void Warn(Marker marker, string msg)
		{
			Warn(msg);
		}

		public void Warn(Marker marker, string format, params object[] arguments)
		{
			Warn(format, arguments);
		}

		public void Warn(Marker marker, string format, object arg)
		{
			Warn(format, arg);
		}

		public void Warn(Marker marker, string msg, Exception t)
		{
			Warn(msg, t);
		}

		public void Warn(Marker marker, string format, object arg1, object arg2)
		{
			Warn(format, arg1, arg2);
		}

		public abstract bool GetIsTraceEnabled(Marker marker);
		public abstract bool GetIsDebugEnabled(Marker marker);
		public abstract bool GetIsInfoEnabled(Marker marker);
		public abstract bool GetIsWarnEnabled(Marker marker);
		public abstract bool GetIsErrorEnabled(Marker marker);
	}
}

