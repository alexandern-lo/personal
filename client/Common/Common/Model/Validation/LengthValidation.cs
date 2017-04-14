using System;
using System.Collections.Generic;

namespace StudioMobile
{
	public class LengthError : ValueError
	{
		public int Min { get; set; }

		public int Max { get; set; }
	}

	public static class LengthValidation
	{
		public const string MinLengthMessage = "{Key} length must be more than {Min}";
		public const string MaxLengthMessage = "{Key} length must be less than {Max}";

		static Check<T> LengthCheck<T> (this Check<T> check, int expected, int actual, string message, bool min)
		{
			var comparisonResult = min ? actual < expected : actual > expected;
			if (comparisonResult) {
				var error = new LengthError { 
					Value = actual,
					MessageFormat = message
				};
				if (min)
					error.Min = expected;
				else
					error.Max = expected;
				check.Fail (error);
			}
			return check;
		}

		public static Check<string> MinLength (this Check<string> check, int length, string message = MinLengthMessage)
		{			
			var actual = check.Value != null ? check.Value.Length : 0;
			return LengthCheck (check, length, actual, message, true);
		}

		public static Check<IList<T>> MinLength<T> (this Check<IList<T>> check, int length, string message = MinLengthMessage)
		{
			var actual = check.Value != null ? check.Value.Count : 0;
			return LengthCheck (check, length, actual, message, true);
		}

		public static Check<string> MaxLength (this Check<string> check, int length, string message = MaxLengthMessage)
		{			
			var actual = check.Value != null ? check.Value.Length : 0;
			return LengthCheck (check, length, actual, message, false);
		}

		public static Check<IList<T>> MaxLength<T> (this Check<IList<T>> check, int length, string message = MaxLengthMessage)
		{
			var actual = check.Value != null ? check.Value.Count : 0;
			return LengthCheck (check, length, actual, message, false);
		}
	}
}

