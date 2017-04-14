using Android.OS;
using Android.Views;
using LiveOakApp.Droid.Services;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using StudioMobile;
using Android.Support.V7.App;
using LiveOakApp.Resources;

namespace LiveOakApp.Droid.Controller
{
	public class MenuProfileFragment : CustomFragment
	{
		
		MenuProfileView view;
		MainMenuViewModel model;
        AlertDialog confirmLogoutDialog;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            model = ((MainActivity)Activity).ViewModel;
            ConfirmLogoutCommand = new Command
            {
                Action = ConfirmLogoutAction
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new MenuProfileView(inflater.Context);

            confirmLogoutDialog = new AlertDialog.Builder(Context)
                                    .SetTitle(L10n.Localize("LogoutTitle", "Logout"))
                                    .SetMessage(L10n.Localize("LogoutMessage", "Do you really want to logout?"))
                                    .SetPositiveButton(L10n.Localize("Ok", "OK"), async (sender, e) => 
            {
                await model.LogoutCommand.ExecuteAsync();
                confirmLogoutDialog.Dismiss();
                NavigateNext();
            })
                                    .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), (sender, e) => confirmLogoutDialog.Dismiss())
                                    .Create();

            Bindings.Property(model, _ => _.UserFullName)
                    .To(view.NameView.TextProperty());

            Bindings.Property(model, _ => _.UserAvatar)
                    .To(view.AvatarView.ImageProperty());

            Bindings.Command(ConfirmLogoutCommand)
                    .To(view.LogoutButton.ClickTarget());

			return view;
		}

        public void NavigateNext()
        {
            DroidNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
        #region Actions
        Command ConfirmLogoutCommand;
        void ConfirmLogoutAction(object args) 
        {
            confirmLogoutDialog.Show();
        }
        #endregion
	}
}

