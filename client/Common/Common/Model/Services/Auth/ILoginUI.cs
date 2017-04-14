
namespace StudioMobile
{
	/// <summary>
	/// Abstracts a way to get Credentials from user. 
	/// Implementation does not have to present GUI but in most cases it will.
	/// </summary>
	public interface ILoginUI<T>
	{
		/// <summary>
		/// Request service to login ServiceSession by requesting authentication information from user.
		/// Implementation must call session.Login or session.Logout or session.CancelAuth at some point. 
		/// Otherwise ServiceSession clients might freeze in EnsureAuthenticated call.
		/// </summary>
		/// <param name="session"></param>
		void Login (ServiceSession<T> session);
	}
}
