using Android.OS;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{
	public class ProfileFragment : CustomFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			model = new ProfileViewModel();
		}

		ProfileView view;
		ProfileViewModel model;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{

			view = new ProfileView(inflater.Context);

			Bindings.Property(model, _ => _.UserFullName)
					.To(view.ProfileName.TextProperty());

			Bindings.Property(model, _ => _.UserAvatar)
			        .To(view.ProfilePhoto.ImageProperty());

			return view;
		}

	}
}

