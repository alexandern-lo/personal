using StudioMobile;
using LiveOakApp.iOS.Controller.Content;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;

namespace LiveOakApp.iOS.Controller
{
    public class ProfileController : MenuContentController<ProfileView>
    {
        readonly ProfileViewModel viewModel;

        public ProfileController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuProfile", "Profile");
            viewModel = new ProfileViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Bindings.Property(viewModel, _ => _.UserAvatar)
                    .To(View.ProfileImageView.ImageProperty());
            Bindings.Property(viewModel, _ => _.UserFullName)
                    .To(View.ProfileNameLabel.TextProperty());
        }
    }
}
