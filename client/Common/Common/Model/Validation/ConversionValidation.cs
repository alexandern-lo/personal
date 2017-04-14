using System;

namespace StudioMobile
{
	public class ValueTypeConversionError : ValueError
	{
		public TypeCode TargetType { get; set; }

		public Exception Exception { get; set; }
	}

	public static class ConversionValidation
	{
		public const string ConversionMessage = "{Key} cannot be converted to {TypeCode}";
		public static Check<T> ConvertTo<T> (this Check<string> check, string message = ConversionMessage, IFormatProvider format = null)
		{
			var typeCode = Type.GetTypeCode (typeof(T));
			ValueTypeConversionError error;
			try {
				var value = Convert.ChangeType (check.Value, typeCode, format);
				return new Check<T> ((T)value, check.Key, check.Errors);
			} catch (InvalidCastException e) {
				//This conversion is not supported
				error = new ValueTypeConversionError { 
					Exception = e
				};
			} catch (FormatException e) {
				//value is not in a format recognized by the typeCode type.
				error = new ValueTypeConversionError { 
					Exception = e
				};
			} catch (OverflowException e) {
				//value represents a number that is out of the range of the typeCode type.
				error = new ValueTypeConversionError { 
					Exception = e
				};
			}
			if (error != null) {
				error.Value = check.Value;
				error.MessageFormat = message;
				check.Fail (error);
			}
			return new Check<T> (default(T), check.Key, check.Errors);
		}

	}
}

