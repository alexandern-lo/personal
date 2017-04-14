using System;

namespace StudioMobile
{
	public class EmptyError : ValueError
	{
	}

	public class NullError : AppError
	{
	}

	public static class EmptyValidations
	{
		public const string NotEmptyMessage = "{Key} expected to be not null and not empty";

		public static Check<string> NotEmpty (this Check<string> check, string message = NotEmptyMessage)
		{
			if (string.IsNullOrWhiteSpace (check.Value)) {
				check.Fail (new EmptyError { MessageFormat = message });
			}
			return check;
		}

		public const string NotNullMessage = "{Key} cannot be null";

		public static Check<T> NotNull<T> (this Check<T> check, string message = NotNullMessage)
		{
			if (ReferenceEquals (check.Value, null)) {
				check.Fail (new NullError { MessageFormat = message });
			}
			return check;
		}

		public const string NullMessage = "{Key} must be null";

		public static Check<T> IsNull<T> (this Check<T> check, string message = NullMessage)
			where T : class
		{
			return check.EqualsTo (null, message);
		}
	}
}

