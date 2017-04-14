using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using LiveOakApp.Models.Services;
using Foundation;

namespace LiveOakApp.iOS.View
{
    public class LoginView : CustomView
    {
        [View(0)]
        public UIImageView BackgroundImageView { get; private set; }

        [View(1)]
        [ButtonSkin("LoginButton")]
        public UIButton LoginButton { get; private set; }

        [View(2)]
        [CommonSkin("ErrorLabel")]
        public UITextView ErrorLabel { get; private set; }

        [View(3)]
        [ButtonSkin("ForgotPasswordButton")]
        public UIButton ResetPasswordButton { get; private set; }

        [View(4)]
        [CommonSkin("ErrorLabel")]
        public UILabel ResetPasswordErrorLabel { get; private set; }

        [View(5)]
        public UIActivityIndicatorView ActivityIndicatorView { get; private set; }


        bool loginRunning = false;
        public bool LoginRunning
        {
            get { return loginRunning; }
            set
            {
                loginRunning = value;
                RefreshActivityIndicator();
            }
        }

        bool resetPasswordRunning;
        public bool ResetPasswordRunning
        {
            get { return resetPasswordRunning; }
            set
            {
                resetPasswordRunning = value;
                RefreshActivityIndicator();
            }
        }

        void RefreshActivityIndicator()
        {
            if (LoginRunning || ResetPasswordRunning)
            {
                ActivityIndicatorView.StartAnimating();
            }
            else
            {
                ActivityIndicatorView.StopAnimating();
            }
        }

        protected override void CreateView()
        {
            base.CreateView();
            BackgroundImageView.Image = UIImage.FromBundle("background");
            BackgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            LoginButton.SetTitle(L10n.Localize("LoginButtonTitle", "Login"), UIControlState.Normal);

            ActivityIndicatorView.HidesWhenStopped = true;

            ResetPasswordButton.Hidden = true; // temporarily hidden
            ResetPasswordErrorLabel.Hidden = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var parentHeight = this.Bounds.Height;
            var parentWidth = this.Bounds.Width;

            BackgroundImageView.Frame = this.Bounds;

            ResetPasswordButton.Frame = this.LayoutFrame()
                .Width(parentWidth)
                .Height(20)
                .Bottom(parentWidth * 0.3f)
                .CenterHorizontally();

            ResetPasswordErrorLabel.Frame = this.LayoutBox()
                .Height(20)
                .Width(parentWidth)
                .Above(ResetPasswordButton, 4);

            LoginButton.Frame = this.LayoutFrame()
                .Width(parentWidth * 0.43f)
                .Height(53)
                .Above(ResetPasswordErrorLabel, 20)
                .CenterHorizontally();

            ErrorLabel.Frame = this.LayoutBox()
                .Height(85)
                .Width(parentWidth)
                .Above(LoginButton, 4)
                .CenterHorizontally();

            ActivityIndicatorView.Frame = this.Bounds;
        }
    }
}
