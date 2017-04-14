using UIKit;
using Foundation;
using StudioMobile;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Resources;
using CoreGraphics;

namespace LiveOakApp.iOS.View.Content
{
    public class PersonsView : CustomView
    {
        [View(0)]
        public ListUpdatingView ListUpdatingView { get; protected set; }

        [View(1)]
        public PersonsMainPartView MainPartView { get; protected set; }

        [View(2)]
        [CommonSkin("BottomPanelBackgroundView")]
        public UIView BottomPanelBackgroundView { get; protected set; }

        [View(3)]
        [CommonSkin("ManualEntryTabBarButton")]
        public CustomTabBarButton ManualEntryTabBarButton { get; protected set; }

        [View(4)]
        [CommonSkin("BarScanTabBarButton")]
        public CustomTabBarButton BarScanTabBarButton { get; protected set; }

        [View(5)]
        [CommonSkin("CardScanTabBarButton")]
        public CustomTabBarButton CardScanTabBarButton { get; protected set; }

        [View(6)]
        public CustomMessageView MessageView { get; protected set; }

        [View(7)]
        public CustomErrorView ErrorView { get; protected set; }

        const float tabBarHeight = 60;
        const float listUpdatingViewHeight = 40;

        protected override void CreateView()
        {
            base.CreateView();
            ErrorView.Hidden = true;
            MessageView.Hidden = true;
        }
        public string EventButtonTitle
        {
            get { return MainPartView.SelectEventButton.Title(UIControlState.Normal); }
            set { MainPartView.SelectEventButton.SetTitle(value, UIControlState.Normal); }
        }

        bool isPersonSelectionMode = false;
        public bool IsPersonSelectionMode
        {
            get { return isPersonSelectionMode; }
            set
            {
                isPersonSelectionMode = value;
                SetNeedsLayout();
            }
        }

        public bool EnableFilterButton
        {
            get
            {
                return MainPartView.EnableFilterButton;
            }
            set
            {
                MainPartView.EnableFilterButton = value;
            }
        }

        bool fetchRunning = false;
        public bool FetchRunning
        {
            get { return fetchRunning; }
            set
            {
                SetNeedsLayout();
                LayoutIfNeeded();

                fetchRunning = value;
                MainPartView.RefreshControl.Subviews[0].Subviews[0].Hidden = value;
                UIView.Animate(0.4, 0, UIViewAnimationOptions.LayoutSubviews | UIViewAnimationOptions.AllowUserInteraction, () => { LayoutSubviews(); }, null);
            }
        }

        public bool SearchRunning
        {
            get { return MainPartView.SearchRunning; }
            set
            {
                MainPartView.SearchRunning = value;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var pW = Bounds.Width;
            var pH = Bounds.Height;

            // TAB BAR
            var tabBarButtonWidth = pW / 3.0f;
            BottomPanelBackgroundView.Frame = this.LayoutBox()
                .Height(tabBarHeight)
                .Left(0)
                .Right(0)
                .Bottom(0);
            CardScanTabBarButton.Frame = this.LayoutBox()
                .Width(tabBarButtonWidth)
                .Height(tabBarHeight)
                .Bottom(0)
                .Left(0);
            BarScanTabBarButton.Frame = this.LayoutBox()
                .Width(tabBarButtonWidth)
                .Height(tabBarHeight)
                .Bottom(0)
                .CenterHorizontally();
            ManualEntryTabBarButton.Frame = this.LayoutBox()
                .Width(tabBarButtonWidth)
                .Height(tabBarHeight)
                .Bottom(0)
                .Right(0);
            ErrorView.Frame = this.LayoutBox()
                .Height(ErrorView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);
            ErrorView.SizeToFit();
            MessageView.Frame = this.LayoutBox()
                .Height(MessageView.Bounds.Height)
                .CenterVertically(-30)
                .Left(15)
                .Right(15);
            MessageView.SizeToFit();

            BarScanTabBarButton.Hidden = IsPersonSelectionMode;
            CardScanTabBarButton.Hidden = IsPersonSelectionMode;
            ManualEntryTabBarButton.Hidden = IsPersonSelectionMode;
            BottomPanelBackgroundView.Hidden = IsPersonSelectionMode;

            if (IsPersonSelectionMode)
            {
                MainPartView.Frame = this.LayoutBox()
                    .Top(FetchRunning ? listUpdatingViewHeight : 0)
                    .Bottom(0)
                    .Left(0)
                    .Right(0);
            }
            else
            {
                MainPartView.Frame = this.LayoutBox()
                    .Top(FetchRunning ? listUpdatingViewHeight : 0)
                    .Above(BottomPanelBackgroundView, 0)
                    .Left(0)
                    .Right(0);
            }

            ListUpdatingView.Frame = this.LayoutBox()
                .Top(FetchRunning ? 0 : -listUpdatingViewHeight)
                .Above(MainPartView, 0)
                .Left(0)
                .Right(0);
        }
    }
}

