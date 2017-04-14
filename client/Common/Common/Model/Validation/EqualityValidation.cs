using System;

namespace StudioMobile
{
	public class EqualityError<T> : AppError
	{
		public T Expected { get; set; }
		public T Actual { get; set; }
	}

	public static class EqualityValidation
	{
		public const string EqualityMessage = "{Key} is not equal to {Expected}";

		public static Check<T> EqualsTo<T> (this Check<T> check, T value, string message = EqualityMessage)			
		{
			if (!Object.Equals (check.Value, value)) {
				check.Fail (new EqualityError<T> {
					MessageFormat = message,
					Expected = value,
					Actual = check.Value
				});
			}
			return check;
		}

		public static Check<bool> IsTrue (this Check<bool> check, string message = EqualityMessage)
		{
			return check.EqualsTo (true, message);
		}

		public static Check<bool> IsFalse (this Check<bool> check, string message = EqualityMessage)
		{
			return check.EqualsTo (false, message);
		}
	}
}

