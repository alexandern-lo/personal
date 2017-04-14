using System;
using System.Threading;
using System.Threading.Tasks;
using SL4N;

namespace StudioMobile
{
	/// <summary>
	/// Session to service which require authentication to be performed. 
	/// ServiceSession interacts with IAuthService and ILoginUI services during authentication procedure. 
	/// See Auth method documentation for details.
	/// </summary>
	/// <typeparam name="CredentialsType">Credentials type to use with ServiceSession</typeparam>
	/// <example>
	/// var api = new RESTAPI();
	/// var session = new ServiceSession<EmailPassword>(new AuthService());
	/// await session.EnsureAuthenticated();
	/// await api.PerformAction1();
	/// </example>
	public class ServiceSession<CredentialsType>
	{
		private static readonly ILogger LOG = LoggerFactory.GetLogger<ServiceSession<CredentialsType>>();

		IAuthService<CredentialsType> authService;
		TaskCompletionSource<bool> loginOperation;
		ILoginUI<CredentialsType> loginUI;

		/// <summary>
		/// Initializes a new instance of the <see cref="StudioMobile.ServiceSession{T}"/> class.
		/// </summary>
		/// <param name="service">Service.</param>
		public ServiceSession (IAuthService<CredentialsType> service)
		{
			Check.Argument (service, "service").NotNull ();
			authService = service;
		}

		const string LoginInProgressError = "Cannot change {Key} while login is in progress";

		/// <summary>
		/// Gets or sets the <see cref="StudioMobile.ILoginUI{T}"/> service to be used during authentication procedure.
		/// </summary>
		protected ILoginUI<CredentialsType> LoginUI {
			get {
				return loginUI;
			}
			set {
				Check.State (loginOperation).IsNull (LoginInProgressError);
				loginUI = value;
			}
		}

		DateTime sessionValidated = DateTime.MinValue;

		/// <summary>
		/// Gets a value indicating whether this session is timed out.
		/// </summary>
		/// <value><c>true</c> if this session timed out; otherwise, <c>false</c>.</value>
		public bool IsSessionTimedOut {
			get { 
				var now = DateTime.Now;
				return (now - sessionValidated) > authService.MaxSessionLifespan;
			}
		}

		/// <summary>
		/// Gets a value indicating whether session can be refreshed without any user intervention.
		/// </summary>
		public bool HasSavedSession {
			get { return authService.GetSessionToken () != null; }
		}

		/// <summary>
		/// Gets a value indicating whether this can present a login UI during authentiction procedure.
		/// </summary>
		public bool HasLoginUI { 
			get { return loginUI != null; }
		}

		/// <summary>
		/// Gets a value indicating whether this session is logged in.
		/// </summary>
		public bool IsLoggedIn {
			get { return authService.IsLoggedIn; }
		}

		/// <summary>
		/// Occurs when session login.
		/// </summary>
		public event EventHandler LoggedIn;
		/// <summary>
		/// Occurs when session logout.
		/// </summary>
		public event EventHandler LoggedOut;

		/// <summary>
		/// Authneticate session with passed credentials and complete authentication procedure if any.
		/// </summary>
		public async Task Login (CredentialsType credentials, CancellationToken token)
		{
			await authService.LoginWithCredentials (credentials, token);
			CompleteAuthOperation (true);
		}

		/// <summary>
		/// Logout session and cancel authentication prcedure if any.
		/// </summary>
		public void Logout ()
		{
			LOG.Trace ("Logout");
			authService.Logout ();
			CancelAuthneticationProcedure ();
			if (LoggedOut != null)
				LoggedOut (this, EventArgs.Empty);
		}

		Task refresh;

		/// <summary>
		/// Authneticate session with passed token and complete authentication procedure if any.
		/// </summary>
		public async Task RefreshSession (CancellationToken cancellationToken)
		{
			if (refresh != null) {
				LOG.Trace ("Waiting for RefreshSession");
				await refresh;
			}
			LOG.Trace ("Start RefreshSession");
			var token = authService.GetSessionToken ();
			refresh = authService.RefreshToken (token, cancellationToken);
			await refresh;
			sessionValidated = DateTime.Now;
			refresh = null;
			CompleteAuthOperation (true);
			LOG.Trace ("Finish RefreshSession");
		}


		/// <summary>
		/// Start authentication procedure if user is not logged in or if session is older than MaxSessionLifespan.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Authentication procedure perform following steps in order
		/// <list type="number">
		/// 	<item>
		/// 		Try to login by refreshing existing session (call to IAuthService.RefreshToken)
		/// 	</item>
		/// 	<item>
		/// 		If refresh token failed then request login with user interaction (call to ILoginUI.Login)
		/// 	</item>
		/// </list>
		/// ILoginUI.Login implementation is expected to finish authentication procedure by calling 
		/// Login, Logout or CancelAuthneticationProcedure.
		/// </para>
		/// <para>
		/// Multiple clients can call EnsureAuthenticated simultaneously. They will get task which will be either 
		/// completed if authentication procedure succeed or faulted with LoginRequiredError if authentication 
		/// cannot be done for some reason.
		/// </para>
		/// </remarks>
		/// <returns>Task to await while authentication procedure is in progress</returns>
		/// <exception cref="LoginRequiredError">
		/// 1. Authorize procedure cancelled by calling CancelAuthneticationProcedure or Logout
		/// 2. RefreshToken failed and credentials service not set.
		/// 3. RefreshToken failed and user cancelled login dialog (ICredentialsService.RequestCredentials() throwed exception).
		/// 4. RefreshToken failed and IAuthService.LoginWithCredentials failed.
		/// </exception>
		public async Task EnsureAuthenticated ()
		{			
			LOG.Trace ("EnsureAuthenticated");
			if (!IsLoggedIn || IsSessionTimedOut) {				
				if (loginOperation == null) {					
					await AuthenticationProcedure ();
				} else {
					LOG.Trace ("Waiting for authorization");
					await loginOperation.Task;
				}
				if (!IsLoggedIn) {
					throw new LoginRequiredError ();
				}
			}
		}

		/// <summary>
		/// Cancel authoentication procedure if it was previously started by EnsureAuthenticated
		/// </summary>
		public void CancelAuthneticationProcedure ()
		{
			CompleteAuthOperation (false);
		}

		async Task AuthenticationProcedure ()
		{
			LOG.Trace ("AuthenticationProcedure");
			Check.State (loginOperation).IsNull ("{Key} cannot be started if other {Key} running.");
			//NOTES to reader
			// While performing login operation loginOperation could be
			// a) stay untouched - if login operation is not interrupted
			// b) null - if CancelAuthorize is called 
			// c) other login operation - in case Authorize called right after CancelAuthorize or Logout
			// to handle these scenarios loginOperation is saved in local 'operation' variable
			var operation = loginOperation = new TaskCompletionSource<bool> ();

			if (HasSavedSession) {
				try {							
					await RefreshSession (CancellationToken.None);
				} catch (Exception e) {							
					LOG.Info ("AuthenticationProcedure - RefreshSession failed", e);
				}
			}
			//We are done if after RefreshSession IsLoggedIn=true even if RefreshSession failed.
			if (AuthProcedureCompleted (operation)) {
				return;
			}

			if (HasLoginUI) {
				try {
					loginUI.Login (this);
					await operation.Task;
				} catch (Exception e) {
					LOG.Warn ("AuthenticationProcedure - unexpected exception failed", e);
				}
			}
		}

		void CompleteAuthOperation (bool result)
		{
			if (loginOperation != null) {
				loginOperation.SetResult (result);
				loginOperation = null;
			}
			if (result && LoggedIn != null)
				LoggedIn (this, EventArgs.Empty);
		}

		bool AuthProcedureCompleted (TaskCompletionSource<bool> operation)
		{			
			return IsLoggedIn || operation != loginOperation;
		}
	}
}