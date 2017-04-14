using System;
using System.Threading;
using System.Threading.Tasks;

namespace StudioMobile
{
	/// <summary>
	/// Implementation of this interface can be used together with <see cref="StudioMobile.ServiceSession{T}"/> to authenticate
	/// user with thirdparty service. Service interface is parametrised with 'CredentialsType' to
	/// permit implementations which does not use username and password to perform authentication. 
	/// For example it could be OS credentials specified during desktop OS login or OAuth token.
	/// </summary>
	/// <typeparam name="CredentialsType">Type of credentials to use.<typeparam>
	public interface IAuthService<CredentialsType>
	{
		/// <summary>
		/// Login the with specified credentials. On success replaces current external service session if any.
 		/// </summary>
		Task LoginWithCredentials (CredentialsType data, CancellationToken token);
		/// <summary>
		/// If auth service allows then this method can be used to refresh authentication token.
		/// without collecting credentials from user. Common scenario use case for OAuth.
		/// </summary>
		Task RefreshToken (string token, CancellationToken cancellationToken);
		/// <summary>
		/// Perform logout. This operation is expected to be quick enough to be performed on synchroniously UI thread.
		/// </summary>
		void Logout ();
		/// <summary>
		/// Gets the session token stored by previous successfull authentications.
		/// </summary>
		/// <returns>The session token or null if no token is stored.</returns>
		string GetSessionToken ();
		/// <summary>
		/// This is a hint to ServiceSession to decide 
		/// </summary>
		/// <value>The max session lifespan.</value>
		TimeSpan MaxSessionLifespan { get; }
		/// <summary>
		/// Determines if there is active and valid session with external service.
		/// </summary>
		/// <value><c>true</c> if this instance is logged in; otherwise, <c>false</c>.</value>
		bool IsLoggedIn { get; }
	}
	
}
