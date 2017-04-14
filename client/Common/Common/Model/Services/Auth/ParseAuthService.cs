using System;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Threading;
using SL4N;

namespace StudioMobile.Parse
{
	/// <summary>
	/// Parse.com authentication service
	/// </summary>
	public abstract class ParseAuthService<T> : IAuthService<T>
	{
		static readonly ILogger LOG = LoggerFactory.GetLogger<ParseAuthService<T>>();

		public virtual async Task LoginWithCredentials (T data, CancellationToken token)
		{
			try {
				await ParseUser.LogInAsync (GetUsername (data), GetPassword (data), token);
			} catch (ParseException e) {
				Throw (e);
			}
		}

		protected abstract string GetUsername (T credentails);

		protected abstract string GetPassword (T credentails);

		protected virtual void Throw (ParseException e)
		{
			LOG.Info("Parse Error", e);
			throw e;
		}

		public virtual async Task RefreshToken (string token, CancellationToken cancellationToken)
		{
			LOG.Info("Refresh token");
			try {
				await ParseUser.BecomeAsync (token, cancellationToken);
			} catch (ParseException e) {
				Throw(e);
			}
		}

		public virtual void Logout ()
		{
			LOG.Info("Logout");
			ParseUser.LogOut ();
		}

		public string GetSessionToken ()
		{
			LOG.Info("GetSessionToken");
			var prop = typeof(ParseUser).GetProperty ("CurrentSessionToken", BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Static);
			return prop.GetValue (null) as string;
		}

		public TimeSpan MaxSessionLifespan {
			get {
				return TimeSpan.FromHours (24);
			}
		}

		public bool IsLoggedIn {
			get {
				return ParseUser.CurrentUser != null && ParseUser.CurrentUser.IsAuthenticated;
			}
		}
	}
}
