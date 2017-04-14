using Android.App;
using Android.OS;
using Android.Content;
using Android.Content.PM;
using Microsoft.Identity.Client;
using StudioMobile;
using LiveOakApp.Models.Services;
using LiveOakApp.Droid.Services;

namespace LiveOakApp.Droid.Controller
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class StartupActivity : CustomActivity
    {
        static readonly string STARTING_STATE = "STARTING_STATE";

        public static void LaunchActivityForState(Activity previousActivity, NavigationManager.RootState state)
        {
            if (previousActivity is StartupActivity)
            {
                ((StartupActivity)previousActivity).ReplaceFragmentToState(state);
                return;
            }
            previousActivity.Finish();
            var intent = new Intent(previousActivity, typeof(StartupActivity));
            intent.PutExtra(STARTING_STATE, (int)state);
            previousActivity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DroidNavigationManager.Instance.Activity = this;
            SetContentView(Resource.Layout.StartupActivity);

            var startingState = (NavigationManager.RootState)Intent.GetIntExtra(STARTING_STATE, (int)NavigationManager.RootState.Startup);
            ReplaceFragmentToState(startingState);
        }

        public Android.Support.V4.App.Fragment FindCurrentFragment()
        {
            return SupportFragmentManager.FindFragmentById(Resource.Id.content_fragment);
        }

        void ReplaceFragmentToState(NavigationManager.RootState state)
        {
            switch (state)
            {
                case NavigationManager.RootState.Initial:
                case NavigationManager.RootState.Startup:
                    ReplaceFragment(new StartupFragment());
                    break;
                case NavigationManager.RootState.Login:
                    ReplaceFragment(new LoginFragment());
                    break;
                case NavigationManager.RootState.Terms:
                    ReplaceFragment(new TermsOfUseFragment());
                    break;
                case NavigationManager.RootState.SubscriptionExpired:
                    ReplaceFragment(new SubscriptionExpiredFragment());
                    break;
                case NavigationManager.RootState.WrongRole:
                    ReplaceFragment(new WrongUserRoleFragment());
                    break;
                default:
                    LOG.Error("Incorrect navigation state: {0}", state);
                    break;
            }
        }

        void ReplaceFragment(Android.Support.V4.App.Fragment newFragment)
        {
            SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_fragment, newFragment)
               .Commit();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}
