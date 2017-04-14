using System;

namespace StudioMobile
{
	public class LoginRequiredError : AppError
	{
		public LoginRequiredError ()
		{
		}

		public LoginRequiredError (string message) : base (message)
		{
		}

		public LoginRequiredError (string message, Exception innerException) : base (message, innerException, false)
		{
		}

	}
}
