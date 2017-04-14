
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models;
using StudioMobile;
using Android.App;
using LiveOakApp.Droid.Services;

namespace LiveOakApp.Droid.Controller
{
    public class WrongUserRoleFragment : CustomFragment
    {

        WrongUserRoleView view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new WrongUserRoleView(inflater.Context);

            view.WebsiteButton.Click += (sender, e) =>
            {
                var browserIntent = new Intent(Intent.ActionView, Uri.Parse("http://portal.avend.co"));
                Context.StartActivity(browserIntent);
            };

            var logoutCommand = new AsyncCommand
            {
                Action = async (arg) =>
                {
                    await ServiceLocator.Instance.AuthService.Logout();
                }
            };

            Bindings.Command(logoutCommand)
                    .To(view.LogoutButton.ClickTarget())
                    .AfterExecute((a, b) => DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded());

            return view;
        }
    }
}
