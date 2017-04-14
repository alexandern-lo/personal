using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Content
{
    public class ProfileView : CustomView
    {
        [View]
        [CommonSkin("AspectFitRemoteImageView")]
        public RemoteImageView ProfileImageView { get; private set; }

        [View]
        [LabelSkin("LargeLightWhiteLabel")]
        public UILabel ProfileNameLabel { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundColor = new UIColor(0.6f, 0.6f, 0.6f, 1.0f);
            ProfileNameLabel.TextAlignment = UITextAlignment.Center;

            ProfileImageView.Placeholder = new Image(UIImage.FromBundle("user-default-image"));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            ProfileNameLabel.SizeToFit();

            ProfileImageView.Frame = this.LayoutBox()
                .Height(Bounds.Height * 0.5f)
                .Width(Bounds.Width * 0.5f)
                .CenterVertically()
                .CenterHorizontally();

            ProfileNameLabel.Frame = this.LayoutBox()
                .Height(ProfileNameLabel.Frame.Height)
                .Width(Bounds.Width)
                .Below(ProfileImageView, 10)
                .CenterHorizontally();
        }
    }
}
