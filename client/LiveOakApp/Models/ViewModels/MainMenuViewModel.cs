using System.Threading;
using System.Threading.Tasks;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class MainMenuViewModel : DataContext
    {
        public enum MainMenuItem
        {
            Dashboard,
            Leads,
            Events,
            Analytics,
            RecentActivity,
            Inbox,
            MyResources,
            Support,
            Profile
        }

        public MainMenuViewModel()
        {
            LogoutCommand = new AsyncCommand
            {
                Action = LogoutAction,
                CanExecute = CanExecuteLogoutAction
            };
        }

        public int InboxCount
        {
            get { return 42; } // example value
        }

        public RemoteImage UserAvatar
        {
            get
            {
                return new RemoteImage(ServiceLocator.Instance.GraphApiService.GetCurrentUserImage);
            }
        }

        public string UserFullName
        {
            get
            {
                return ServiceLocator.Instance.AuthService.CurrentUser.FullName;
            }
        }

        #region Logout

        public AsyncCommand LogoutCommand { get; private set; }

        async Task LogoutAction(object arg)
        {
            await ServiceLocator.Instance.AuthService.Logout();
        }

        bool CanExecuteLogoutAction(object arg)
        {
            return !LogoutCommand.IsRunning;
        }

        #endregion

        #region Lead overwrite dialog

        public async Task<LeadOverwriteViewModel> FindFirstLeadForOverwrite(CancellationToken? cancellationToken)
        {
            ServiceLocator.Instance.ProfileService.UpdatedProfileIfNeeded();

            var lead = await ServiceLocator.Instance.LeadsService.FindOldestLeadForOverwriteDecision(cancellationToken);
            if (lead == null) return null;
            return new LeadOverwriteViewModel(lead);
        }

        #endregion
    }
}
