using System;

namespace StudioMobile
{
	public class RangeError<T> : ValueError 
		where T : struct, IComparable
	{
		public T? Min { get; set; }

		public T? Max { get; set; }
	}

	public static class NumberValidation
	{
		public const string NumberMessage = "{Key} is not a number";


		public const string MoreThanMessage = "{Key} expected to be more than {Min}";

		public static Check<T> MoreThan<T> (this Check<T> check, T expected, string message = MoreThanMessage)
			where T : struct, IComparable
		{
			if (check.Value.CompareTo (expected) < 0) {
				check.Fail (new RangeError<T> {
					MessageFormat = message,
					Min = expected
				});
			}
			return check;
		}

		public const string LessThanMessage = "{Key} expected to be less than {Max}";

		public static Check<T> LessThan<T> (this Check<T> check, T expected, string message = MoreThanMessage)
			where T : struct, IComparable
		{
			if (expected.CompareTo (check.Value) > 0) {
				check.Fail (new RangeError<T> {
					MessageFormat = message,
					Max = expected
				});
			}
			return check;
		}

		public const string BetweenMessage = "{Key} expected to be in between {Min} and {Max}";

		public static Check<T> Between<T> (this Check<T> check, T min, T max, string message = BetweenMessage)
			where T : struct, IComparable
		{
			if (min.CompareTo (check.Value) < 0 || max.CompareTo (check.Value) > 0) {
				check.Fail (new RangeError<T> {
					MessageFormat = message,
					Max = max,
					Min = min
				});
			}
			return check;
		}


	}
}

