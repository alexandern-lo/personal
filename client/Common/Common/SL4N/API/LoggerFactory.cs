using System;

namespace SL4N
{
	public class LoggerFactory
	{
		/**
	     * Return a logger named according to the name parameter using the
	     * statically bound {@link ILoggerFactory} instance.
	     * 
	     * @param name
	     *            The name of the logger.
	     * @return logger
	     */
		public static ILogger GetLogger(String name)
		{
			if (Adapter == null)
			{
				throw new InvalidOperationException("Adapter is null.");
			}
			return Adapter.GetLogger(name);
		}

		/**
		 * Return a logger named corresponding to the class passed as parameter,
		 * using the statically bound {@link ILoggerFactory} instance.
		 * 
		 * @param clazz
		 *            the returned logger will be named after clazz
		 * @return logger
		 * 
		 */
		public static ILogger GetLogger(Type clazz)
		{
			return GetLogger(clazz.Name);
		}

		public static ILogger GetLogger<T>()
		{
			return GetLogger(typeof(T).Name);
		}

		public static ILoggerFactory Adapter { get; set; }
	}
}

