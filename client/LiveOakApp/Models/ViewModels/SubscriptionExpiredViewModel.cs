using System.Threading.Tasks;
using StudioMobile;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.ViewModels
{
    public class SubscriptionExpiredViewModel : DataContext
    {
        readonly ProfileService service;

        public SubscriptionExpiredViewModel()
        {
            service = ServiceLocator.Instance.ProfileService;

            RecheckCommand = new AsyncCommand()
            {
                Action = RecheckAction,
                CanExecute = CanExecuteRecheckAction
            };

            LogoutCommand = new AsyncCommand()
            {
                Action = LogoutAction,
                CanExecute = CanExecuteLogoutAction
            };
        }

        public bool IsSubscriptionValid { get { return service.IsSubscriptionValid; } }

        public AsyncCommand RecheckCommand { get; private set; }

        async Task RecheckAction(object arg)
        {
            await service.GetProfile(null);
            RaisePropertyChanged(() => IsSubscriptionValid);
        }

        bool CanExecuteRecheckAction(object arg)
        {
            return !LogoutCommand.IsRunning && !RecheckCommand.IsRunning;
        }

        public AsyncCommand LogoutCommand { get; private set; }

        async Task LogoutAction(object args)
        {
            await ServiceLocator.Instance.AuthService.Logout();
        }

        bool CanExecuteLogoutAction(object arg)
        {
            return !LogoutCommand.IsRunning && !RecheckCommand.IsRunning;
        }
    }
}
