using SL4N;
using Microsoft.Identity.Client;

#if __IOS__
using LiveOakApp.iOS.Services;
#endif
#if __ANDROID__
using LiveOakApp.Droid.Services;
#endif

namespace LiveOakApp.Models.Services
{
    public abstract class NavigationManager
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<NavigationManager>();

        public static NavigationManager Instance
        {
            get
            {
#if __IOS__
                return IOSNavigationManager.Instance;
#endif
#if __ANDROID__
                return DroidNavigationManager.Instance;
#endif
            }
        }

        public abstract IPlatformParameters CurrentPlatformParameters();

        public enum RootState
        {
            Initial,
            SubscriptionExpired,
            WrongRole,
            Startup,
            Login,
            Terms,
            Main
        }

        public RootState GetRequiredState()
        {
            if (!ServiceLocator.Instance.AuthService.IsLoggedIn)
                return RootState.Login;
            if (!ServiceLocator.Instance.ProfileService.ProfileRequest.DataIsLoadedToCache)
                return RootState.Startup;
            if (!ServiceLocator.Instance.ProfileService.IsSubscriptionValid)
                return RootState.SubscriptionExpired;
            if (ServiceLocator.Instance.ProfileService.IsSuperAdmin)
                return RootState.WrongRole;
            if (!ServiceLocator.Instance.TermsOfUseService.TermsRequest.DataIsLoadedToCache)
                return RootState.Startup;
            if (!ServiceLocator.Instance.TermsOfUseService.IsAccepted)
                return RootState.Terms;
            return RootState.Main;
        }

        protected abstract RootState GetCurrentState();

        protected abstract void ChangeState(RootState oldState, RootState newState);

        public void NavigateToRequiredStateIfNeeded()
        {
            var currentState = GetCurrentState();
            var requiredState = GetRequiredState();
            LOG.Debug(string.Format("NavigateToCurrentStateIfNeeded() currentState: {0}, requiredState: {1}", currentState, requiredState));
            if (currentState == requiredState) return;
            ChangeState(currentState, requiredState);

            ServiceLocator.Instance.PreloadService.StartPreloadingIfNeeded();
        }
    }
}
