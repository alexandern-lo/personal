using Android.App;
using LiveOakApp.Models.Services;
using LiveOakApp.Droid.Controller;
using Microsoft.Identity.Client;

namespace LiveOakApp.Droid.Services
{
    public class DroidNavigationManager : NavigationManager
    {
        public static new DroidNavigationManager Instance = new DroidNavigationManager();

        public Activity Activity { get; set; }

        public override IPlatformParameters CurrentPlatformParameters()
        {
            if (Activity == null) return null;
            return new PlatformParameters(Activity);
        }

        protected override RootState GetCurrentState()
        {
            if (Activity is MainActivity)
                return RootState.Main;
            var startupActivity = Activity as StartupActivity;
            if (startupActivity != null)
            {
                var currentFragment = startupActivity.FindCurrentFragment();
                if (currentFragment is StartupFragment)
                    return RootState.Startup;
                if (currentFragment is LoginFragment)
                    return RootState.Login;
                if (currentFragment is TermsOfUseFragment)
                    return RootState.Terms;
                if (currentFragment is SubscriptionExpiredFragment)
                    return RootState.SubscriptionExpired;
                if (currentFragment is WrongUserRoleFragment)
                    return RootState.WrongRole;
            }
            return RootState.Initial;
        }

        protected override void ChangeState(RootState oldState, RootState newState)
        {
            switch (newState)
            {
                case RootState.Initial:
                    break;
                case RootState.Main:
                        MainActivity.LaunchActivity(Activity);
                    break;
                case RootState.Startup:
                case RootState.Login:
                case RootState.SubscriptionExpired:
                    StartupActivity.LaunchActivityForState(Activity, newState);
                    break;
                case RootState.WrongRole:
                    StartupActivity.LaunchActivityForState(Activity, newState);
                    break;
                case RootState.Terms:
                    StartupActivity.LaunchActivityForState(Activity, newState);
                    break;
            }
        }
    }
}
