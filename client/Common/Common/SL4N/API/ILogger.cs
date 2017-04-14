using System;

namespace SL4N
{
	public interface ILogger
	{
		/**
		 * Return the name of this <code>Logger</code> instance.
		 * @return name of this logger instance 
		 */
		string Name { get; }

		/**
		 * Is the logger instance enabled for the TRACE level?
		 *
		 * @return True if this Logger is enabled for the TRACE level,
		 *         false otherwise.
		 * @since 1.4
		 */
		bool IsTraceEnabled { get; }

		/**
		 * Log a message at the TRACE level.
		 *
		 * @param msg the message string to be logged
		 * @since 1.4
		 */
		void Trace(String msg);

		/**
		 * Log a message at the TRACE level according to the specified format
		 * and argument.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the TRACE level. </p>
		 *
		 * @param format the format string
		 * @param arg    the argument
		 * @since 1.4
		 */
		void Trace(String format, Object arg);

		/**
		 * Log a message at the TRACE level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the TRACE level. </p>
		 *
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 * @since 1.4
		 */
		void Trace(String format, Object arg1, Object arg2);

		/**
		 * Log a message at the TRACE level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous string concatenation when the logger
		 * is disabled for the TRACE level. However, this variant incurs the hidden
		 * (and relatively small) cost of creating an <code>Object[]</code> before invoking the method,
		 * even if this logger is disabled for TRACE. The variants taking {@link #Trace(String, Object) one} and
		 * {@link #Trace(String, Object, Object) two} arguments exist solely in order to avoid this hidden cost.</p>
		 *
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 * @since 1.4
		 */
		void Trace(String format, params Object[] arguments);

		/**
		 * Log an exception (throwable) at the TRACE level with an
		 * accompanying message.
		 *
		 * @param msg the message accompanying the exception
		 * @param t   the exception (throwable) to log
		 * @since 1.4
		 */
		void Trace(String msg, Exception t);

		/**
		 * Similar to {@link #isTraceEnabled()} method except that the
		 * marker data is also taken into account.
		 *
		 * @param marker The marker data to take into consideration
		 * @return True if this Logger is enabled for the TRACE level,
		 *         false otherwise.
		 *         
		 * @since 1.4
		 */
		bool GetIsTraceEnabled(Marker marker);

		/**
		 * Log a message with the specific Marker at the TRACE level.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param msg    the message string to be logged
		 * @since 1.4
		 */
		void Trace(Marker marker, String msg);

		/**
		 * This method is similar to {@link #Trace(String, Object)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg    the argument
		 * @since 1.4
		 */
		void Trace(Marker marker, String format, Object arg);

		/**
		 * This method is similar to {@link #Trace(String, Object, Object)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 * @since 1.4
		 */
		void Trace(Marker marker, String format, Object arg1, Object arg2);

		/**
		 * This method is similar to {@link #Trace(String, params Object[] )}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker   the marker data specific to this log statement
		 * @param format   the format string
		 * @param argArray an array of arguments
		 * @since 1.4
		 */
		void Trace(Marker marker, String format, params Object[] argArray);

		/**
		 * This method is similar to {@link #Trace(String, Exception)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param msg    the message accompanying the exception
		 * @param t      the exception (throwable) to log
		 * @since 1.4
		 */
		void Trace(Marker marker, String msg, Exception t);

		/**
		 * Is the logger instance enabled for the DEBUG level?
		 *
		 * @return True if this Logger is enabled for the DEBUG level,
		 *         false otherwise.
		 */
		bool IsDebugEnabled { get; }

		/**
		 * Log a message at the DEBUG level.
		 *
		 * @param msg the message string to be logged
		 */
		void Debug(String msg);

		/**
		 * Log a message at the DEBUG level according to the specified format
		 * and argument.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the DEBUG level. </p>
		 *
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Debug(String format, Object arg);

		/**
		 * Log a message at the DEBUG level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the DEBUG level. </p>
		 *
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Debug(String format, Object arg1, Object arg2);

		/**
		 * Log a message at the DEBUG level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous string concatenation when the logger
		 * is disabled for the DEBUG level. However, this variant incurs the hidden
		 * (and relatively small) cost of creating an <code>Object[]</code> before invoking the method,
		 * even if this logger is disabled for DEBUG. The variants taking
		 * {@link #Debug(String, Object) one} and {@link #Debug(String, Object, Object) two}
		 * arguments exist solely in order to avoid this hidden cost.</p>
		 *
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Debug(String format, params Object[] arguments);

		/**
		 * Log an exception (throwable) at the DEBUG level with an
		 * accompanying message.
		 *
		 * @param msg the message accompanying the exception
		 * @param t   the exception (throwable) to log
		 */
		void Debug(String msg, Exception t);

		/**
		 * Similar to {@link #isDebugEnabled()} method except that the
		 * marker data is also taken into account.
		 *
		 * @param marker The marker data to take into consideration
		 * @return True if this Logger is enabled for the DEBUG level,
		 *         false otherwise. 
		 */
		bool GetIsDebugEnabled(Marker marker);

		/**
		 * Log a message with the specific Marker at the DEBUG level.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param msg    the message string to be logged
		 */
		void Debug(Marker marker, String msg);

		/**
		 * This method is similar to {@link #Debug(String, Object)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Debug(Marker marker, String format, Object arg);

		/**
		 * This method is similar to {@link #Debug(String, Object, Object)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Debug(Marker marker, String format, Object arg1, Object arg2);

		/**
		 * This method is similar to {@link #Debug(String, params Object[] )}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker    the marker data specific to this log statement
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Debug(Marker marker, String format, params Object[] arguments);

		/**
		 * This method is similar to {@link #Debug(String, Exception)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param msg    the message accompanying the exception
		 * @param t      the exception (throwable) to log
		 */
		void Debug(Marker marker, String msg, Exception t);

		/**
		 * Is the logger instance enabled for the INFO level?
		 *
		 * @return True if this Logger is enabled for the INFO level,
		 *         false otherwise.
		 */
		bool IsInfoEnabled { get; }

		/**
		 * Log a message at the INFO level.
		 *
		 * @param msg the message string to be logged
		 */
		void Info(String msg);

		/**
		 * Log a message at the INFO level according to the specified format
		 * and argument.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the INFO level. </p>
		 *
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Info(String format, Object arg);

		/**
		 * Log a message at the INFO level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the INFO level. </p>
		 *
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Info(String format, Object arg1, Object arg2);

		/**
		 * Log a message at the INFO level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous string concatenation when the logger
		 * is disabled for the INFO level. However, this variant incurs the hidden
		 * (and relatively small) cost of creating an <code>Object[]</code> before invoking the method,
		 * even if this logger is disabled for INFO. The variants taking
		 * {@link #Info(String, Object) one} and {@link #Info(String, Object, Object) two}
		 * arguments exist solely in order to avoid this hidden cost.</p>
		 *
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Info(String format, params Object[] arguments);

		/**
		 * Log an exception (throwable) at the INFO level with an
		 * accompanying message.
		 *
		 * @param msg the message accompanying the exception
		 * @param t   the exception (throwable) to log
		 */
		void Info(String msg, Exception t);

		/**
		 * Similar to {@link #isInfoEnabled()} method except that the marker
		 * data is also taken into consideration.
		 *
		 * @param marker The marker data to take into consideration
		 * @return true if this logger is warn enabled, false otherwise 
		 */
		bool GetIsInfoEnabled(Marker marker);

		/**
		 * Log a message with the specific Marker at the INFO level.
		 *
		 * @param marker The marker specific to this log statement
		 * @param msg    the message string to be logged
		 */
		void Info(Marker marker, String msg);

		/**
		 * This method is similar to {@link #Info(String, Object)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Info(Marker marker, String format, Object arg);

		/**
		 * This method is similar to {@link #Info(String, Object, Object)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Info(Marker marker, String format, Object arg1, Object arg2);

		/**
		 * This method is similar to {@link #Info(String, params Object[] )}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker    the marker data specific to this log statement
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Info(Marker marker, String format, params Object[] arguments);

		/**
		 * This method is similar to {@link #Info(String, Exception)} method
		 * except that the marker data is also taken into consideration.
		 *
		 * @param marker the marker data for this log statement
		 * @param msg    the message accompanying the exception
		 * @param t      the exception (throwable) to log
		 */
		void Info(Marker marker, String msg, Exception t);

		/**
		 * Is the logger instance enabled for the WARN level?
		 *
		 * @return True if this Logger is enabled for the WARN level,
		 *         false otherwise.
		 */
		bool IsWarnEnabled { get; }

		/**
		 * Log a message at the WARN level.
		 *
		 * @param msg the message string to be logged
		 */
		void Warn(String msg);

		/**
		 * Log a message at the WARN level according to the specified format
		 * and argument.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the WARN level. </p>
		 *
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Warn(String format, Object arg);

		/**
		 * Log a message at the WARN level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous string concatenation when the logger
		 * is disabled for the WARN level. However, this variant incurs the hidden
		 * (and relatively small) cost of creating an <code>Object[]</code> before invoking the method,
		 * even if this logger is disabled for WARN. The variants taking
		 * {@link #Warn(String, Object) one} and {@link #Warn(String, Object, Object) two}
		 * arguments exist solely in order to avoid this hidden cost.</p>
		 *
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Warn(String format, params Object[] arguments);

		/**
		 * Log a message at the WARN level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the WARN level. </p>
		 *
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Warn(String format, Object arg1, Object arg2);

		/**
		 * Log an exception (throwable) at the WARN level with an
		 * accompanying message.
		 *
		 * @param msg the message accompanying the exception
		 * @param t   the exception (throwable) to log
		 */
		void Warn(String msg, Exception t);

		/**
		 * Similar to {@link #isWarnEnabled()} method except that the marker
		 * data is also taken into consideration.
		 *
		 * @param marker The marker data to take into consideration
		 * @return True if this Logger is enabled for the WARN level,
		 *         false otherwise.
		 */
		bool GetIsWarnEnabled(Marker marker);

		/**
		 * Log a message with the specific Marker at the WARN level.
		 *
		 * @param marker The marker specific to this log statement
		 * @param msg    the message string to be logged
		 */
		void Warn(Marker marker, String msg);

		/**
		 * This method is similar to {@link #Warn(String, Object)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Warn(Marker marker, String format, Object arg);

		/**
		 * This method is similar to {@link #Warn(String, Object, Object)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Warn(Marker marker, String format, Object arg1, Object arg2);

		/**
		 * This method is similar to {@link #Warn(String, params Object[] )}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker    the marker data specific to this log statement
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Warn(Marker marker, String format, params Object[] arguments);

		/**
		 * This method is similar to {@link #Warn(String, Exception)} method
		 * except that the marker data is also taken into consideration.
		 *
		 * @param marker the marker data for this log statement
		 * @param msg    the message accompanying the exception
		 * @param t      the exception (throwable) to log
		 */
		void Warn(Marker marker, String msg, Exception t);

		/**
		 * Is the logger instance enabled for the ERROR level?
		 *
		 * @return True if this Logger is enabled for the ERROR level,
		 *         false otherwise.
		 */
		bool IsErrorEnabled { get; }

		/**
		 * Log a message at the ERROR level.
		 *
		 * @param msg the message string to be logged
		 */
		void Error(String msg);

		/**
		 * Log a message at the ERROR level according to the specified format
		 * and argument.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the ERROR level. </p>
		 *
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Error(String format, Object arg);

		/**
		 * Log a message at the ERROR level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous object creation when the logger
		 * is disabled for the ERROR level. </p>
		 *
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Error(String format, Object arg1, Object arg2);

		/**
		 * Log a message at the ERROR level according to the specified format
		 * and arguments.
		 * <p/>
		 * <p>This form avoids superfluous string concatenation when the logger
		 * is disabled for the ERROR level. However, this variant incurs the hidden
		 * (and relatively small) cost of creating an <code>Object[]</code> before invoking the method,
		 * even if this logger is disabled for ERROR. The variants taking
		 * {@link #Error(String, Object) one} and {@link #Error(String, Object, Object) two}
		 * arguments exist solely in order to avoid this hidden cost.</p>
		 *
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Error(String format, params Object[] arguments);

		/**
		 * Log an exception (throwable) at the ERROR level with an
		 * accompanying message.
		 *
		 * @param msg the message accompanying the exception
		 * @param t   the exception (throwable) to log
		 */
		void Error(String msg, Exception t);

		/**
		 * Similar to {@link #isErrorEnabled()} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker The marker data to take into consideration
		 * @return True if this Logger is enabled for the ERROR level,
		 *         false otherwise.
		 */
		bool GetIsErrorEnabled(Marker marker);

		/**
		 * Log a message with the specific Marker at the ERROR level.
		 *
		 * @param marker The marker specific to this log statement
		 * @param msg    the message string to be logged
		 */
		void Error(Marker marker, String msg);

		/**
		 * This method is similar to {@link #Error(String, Object)} method except that the
		 * marker data is also taken into consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg    the argument
		 */
		void Error(Marker marker, String format, Object arg);

		/**
		 * This method is similar to {@link #Error(String, Object, Object)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param format the format string
		 * @param arg1   the first argument
		 * @param arg2   the second argument
		 */
		void Error(Marker marker, String format, Object arg1, Object arg2);

		/**
		 * This method is similar to {@link #Error(String, params Object[] )}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker    the marker data specific to this log statement
		 * @param format    the format string
		 * @param arguments a list of 3 or more arguments
		 */
		void Error(Marker marker, String format, params Object[] arguments);

		/**
		 * This method is similar to {@link #Error(String, Exception)}
		 * method except that the marker data is also taken into
		 * consideration.
		 *
		 * @param marker the marker data specific to this log statement
		 * @param msg    the message accompanying the exception
		 * @param t      the exception (throwable) to log
		 */
		void Error(Marker marker, String msg, Exception t);
	}
}

