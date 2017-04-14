using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.View
{
    public class SubscriptionExpiredView : CustomView
    {
        [View(0)]
        [CommonSkin("BackgroundImageView")]
        public UIImageView BackgroundImage { get; private set; }

        [View(1)]
        [ButtonSkin("SubscriptionExpiredRecheckButton")]
        public UIButton RecheckButton { get; private set; }

        [View(2)]
        [ButtonSkin("SubscriptionExpiredLogoutButton")]
        public UIButton LogoutButton { get; private set; }

        [View(4)]
        [CommonSkin("SubscriptionExpiredLabel")]
        public UILabel MessageLabel { get; private set; }

        [View(5)]
        public ActivityDialogView ActivityView { get; private set; }

        [View(6)]
        [ButtonSkin("SubscriptionRenewButton")]
        public UIButton RenewSubscriptionButton { get; private set; }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var margin = 20;
            var buttonSpacing = 5;
            var buttonWidth = this.Bounds.Width / 2 - margin - buttonSpacing;

            BackgroundImage.Frame = Bounds;

            RecheckButton.Frame = this.LayoutBox()
                .Height(50)
                .Width(buttonWidth)
                .Bottom(margin)
                .Right(margin);

            LogoutButton.Frame = this.LayoutBox()
                .Height(50)
                .Width(buttonWidth)
                .Bottom(margin)
                .Left(margin);

            MessageLabel.Frame = this.LayoutBox()
                .Height(MessageLabel.SizeThatFits(Bounds.Size).Height)
                .Width(this.Bounds.Width)
                .CenterVertically()
                .CenterHorizontally();

            RenewSubscriptionButton.Frame = this.LayoutBox()
                .Height(40)
                .Width(200)
                .Below(MessageLabel, 15)
                .CenterHorizontally();

            ActivityView.Frame = this.LayoutBox()
                .Width(ActivityView.Bounds.Width)
                .Height(ActivityView.Bounds.Height)
                .CenterHorizontally()
                .Below(MessageLabel, 30);
        }

        bool recheckRunning;
        public bool RecheckRunning
        {
            get { return recheckRunning; }
            set
            {
                recheckRunning = value;
                RefreshActivityIndicator();
            }
        }

        void RefreshActivityIndicator()
        {
            var showActivity = recheckRunning;
            RenewSubscriptionButton.Hidden = recheckRunning;
            ActivityView.Hidden = !showActivity;
        }
    }
}
